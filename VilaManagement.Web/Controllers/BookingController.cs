using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;
using VilaManagement.Application.Common;
using VilaManagement.Application.Common.Interfaces;
using VilaManagement.Domain.Entities;

namespace VilaManagement.Web.Controllers
{
    public class BookingController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BookingController(IUnitOfWork unitOfWork,
            IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        [Authorize]
        public IActionResult Finalize(int vilaId, DateOnly checkInDate, int numberOfNights)
        {
            var userId = User.Identity switch
            {
                ClaimsIdentity { IsAuthenticated: true } ci => ci.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                _ => throw new KeyNotFoundException()
            };

            ApplicationUser user = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

            Booking booking = new Booking
            {
                VilaId = vilaId,
                Vila = _unitOfWork.Villa.Get(v => v.Id == vilaId, includedProperties: new string[] { "Amenities" }),
                CheckInDate = checkInDate,
                Nights = numberOfNights,
                CheckOutDate = checkInDate.AddDays(numberOfNights),
                UserId = userId,
                Phone = user.PhoneNumber,
                Email = user.Email,
                Name = user.Name
            };
            booking.TotalCost = booking.Vila.Price * numberOfNights;
            return View(booking);
        }


        [Authorize]
        [HttpPost]
        public IActionResult Finalize(Booking booking)
        {
            var vila = _unitOfWork.Villa.Get(v => v.Id == booking.VilaId, includedProperties: new string[] { "Amenities" });
            booking.TotalCost = vila.Price * booking.Nights;

            booking.OrderStatus = BookingOrderStatus.Pending;
            booking.BookingDate = DateTime.Now;

            _unitOfWork.Booking.Add(booking);
            _unitOfWork.SaveChanges();

            //if (!_villaService.IsVillaAvailableByDate(villa.Id, booking.Nights, booking.CheckInDate))
            //{
            //    TempData["error"] = "Room has been sold out!";
            //    //no rooms available
            //    return RedirectToAction(nameof(FinalizeBooking), new
            //    {
            //        villaId = booking.VillaId,
            //        checkInDate = booking.CheckInDate,
            //        nights = booking.Nights
            //    });
            //}




            //_bookingService.CreateBooking(booking);

            //var domain = Request.Scheme + "://" + Request.Host.Value + "/";

            //var options = new SessionCreateOptions
            //{
            //    LineItems = new List<SessionLineItemOptions>(),
            //    Mode = "payment",
            //    SuccessUrl = $"{domain}booking/confirmation?booking={booking.Id}.html",
            //    CancelUrl = $"{domain}booking/finalize?vilaId={vila.Id}&checkInDate={booking.CheckInDate}&numberOfNights={booking.Nights} .html",
            //};
            //options.LineItems.Add(new SessionLineItemOptions
            //{
            //    PriceData = new SessionLineItemPriceDataOptions
            //    {
            //        UnitAmount = (long)booking.TotalCost * 100,
            //        Currency = "usd",
            //        ProductData = new SessionLineItemPriceDataProductDataOptions
            //        {
            //            Name = vila.Name,
            //            //       Images = new List<string> { domain + vila.ImageUrl }
            //        },
            //    },
            //    Quantity = 1,
            //});

            //var service = new SessionService();
            //Session session = service.Create(options);

            //_unitOfWork.Booking.UpdateStripePaymentId(booking.Id, session.Id, session.PaymentIntentId);
            //_unitOfWork.SaveChanges();

            //Response.Headers.Add("Location", session.Url);
            //return new StatusCodeResult(303);

            return RedirectToAction(nameof(Confirmation), new { bookingId = booking.Id });

            //var options = _paymentService.CreateStripeSessionOptions(booking, villa, domain);

            //var session = _paymentService.CreateStripeSession(options);

            //_unitOfWork.Booking.UpdateStripePaymentId(booking.Id, session.Id, session.PaymentIntentId);

        }

        [Authorize]
        public IActionResult Confirmation(int bookingId)
        {
            var booking = _unitOfWork.Booking.Get(b => b.Id == bookingId, new string[] { nameof(Booking.Vila), nameof(Booking.User) });
            if (booking is Booking { OrderStatus: BookingOrderStatus.Pending })
            {
                //    var service = new SessionService();

                //    var session = service.Get(booking.StripeSessionId);
                //    if (session.PaymentStatus == "paid")
                //    {
                //        _unitOfWork.Booking.UpdateOrderStatus(bookingId, BookingOrderStatus.Approved);
                //        _unitOfWork.Booking.UpdateStripePaymentId(bookingId, session.Id, session.PaymentIntentId);
                //        _unitOfWork.SaveChanges();
                //    }
            }

            return View(bookingId);
        }

        [HttpPost]
        public IActionResult GenerateInvoice(int id)
        {
            var wwwroot = _webHostEnvironment.WebRootPath;
            string dataPath = $"{wwwroot}/exports/BookingDetails.docx";

            CreateDocxWithTable(dataPath);
            var fileStream = System.IO.File.OpenRead(dataPath);

            return File(fileStream, "application/docx", "BookingDetails.docx");

        }

        // Create a DOCX with formatted table
        public void CreateDocxWithTable(string filePath)
        {
            using (WordprocessingDocument doc = WordprocessingDocument.Create(
                filePath, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = doc.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body body = mainPart.Document.AppendChild(new Body());

                // Add a title
                Paragraph title = body.AppendChild(new Paragraph());
                Run titleRun = title.AppendChild(new Run());
                titleRun.AppendChild(new Text("Sales Report 2024"));

                // Format title (bold, large font)
                RunProperties titleProps = titleRun.InsertAt(new RunProperties(), 0);
                titleProps.AppendChild(new Bold());
                titleProps.AppendChild(new FontSize { Val = "32" }); // 16pt

                // Add spacing after title
                title.AppendChild(new ParagraphProperties(
                    new SpacingBetweenLines { After = "240" }));

                // Create table
                Table table = new Table();

                // Define table properties (borders, width)
                TableProperties tblProps = new TableProperties(
                    new TableBorders(
                        new TopBorder { Val = BorderValues.Single, Size = 12 },
                        new BottomBorder { Val = BorderValues.Single, Size = 12 },
                        new LeftBorder { Val = BorderValues.Single, Size = 12 },
                        new RightBorder { Val = BorderValues.Single, Size = 12 },
                        new InsideHorizontalBorder { Val = BorderValues.Single, Size = 6 },
                        new InsideVerticalBorder { Val = BorderValues.Single, Size = 6 }
                    ),
                    new TableWidth { Width = "5000", Type = TableWidthUnitValues.Pct }
                );
                table.AppendChild(tblProps);

                // Create header row
                TableRow headerRow = new TableRow();

                // Add header cells with formatting
                headerRow.Append(
                    CreateTableCell("Product", true, "B4C6E7"),
                    CreateTableCell("Q1 Sales", true, "B4C6E7"),
                    CreateTableCell("Q2 Sales", true, "B4C6E7"),
                    CreateTableCell("Total", true, "B4C6E7")
                );
                table.AppendChild(headerRow);

                // Add data rows
                table.AppendChild(CreateDataRow("Laptop", "$45,000", "$52,000", "$97,000", false));
                table.AppendChild(CreateDataRow("Desktop", "$32,000", "$28,000", "$60,000", true));
                table.AppendChild(CreateDataRow("Monitor", "$18,000", "$22,000", "$40,000", false));
                table.AppendChild(CreateDataRow("Keyboard", "$8,500", "$9,200", "$17,700", true));

                // Add total row with different formatting
                TableRow totalRow = new TableRow();
                totalRow.Append(
                    CreateTableCell("TOTAL", true, "FFC000"),
                    CreateTableCell("$103,500", true, "FFC000"),
                    CreateTableCell("$111,200", true, "FFC000"),
                    CreateTableCell("$214,700", true, "FFC000")
                );
                table.AppendChild(totalRow);

                body.AppendChild(table);

                // Add a note after the table
                Paragraph note = body.AppendChild(new Paragraph());
                note.AppendChild(new ParagraphProperties(
                    new SpacingBetweenLines { Before = "240" }));
                Run noteRun = note.AppendChild(new Run());
                noteRun.AppendChild(new Text("Note: All figures are in USD"));
                RunProperties noteProps = noteRun.InsertAt(new RunProperties(), 0);
                noteProps.AppendChild(new Italic());
                noteProps.AppendChild(new Color { Val = "808080" });

                mainPart.Document.Save();

            }
        }

        // Create a table cell with formatting
        private TableCell CreateTableCell(string text, bool isBold, string bgColor = null)
        {
            TableCell cell = new TableCell();

            // Set cell properties (shading/background color)
            TableCellProperties cellProps = new TableCellProperties();

            if (!string.IsNullOrEmpty(bgColor))
            {
                cellProps.Append(new Shading
                {
                    Val = ShadingPatternValues.Clear,
                    Fill = bgColor
                });
            }

            // Add padding
            cellProps.Append(new TableCellMargin(
                new TopMargin { Width = "100", Type = TableWidthUnitValues.Dxa },
                new BottomMargin { Width = "100", Type = TableWidthUnitValues.Dxa },
                new LeftMargin { Width = "100", Type = TableWidthUnitValues.Dxa },
                new RightMargin { Width = "100", Type = TableWidthUnitValues.Dxa }
            ));

            cell.Append(cellProps);

            // Add paragraph with text
            Paragraph para = new Paragraph();
            Run run = para.AppendChild(new Run());
            run.AppendChild(new Text(text));

            // Format text
            RunProperties runProps = run.InsertAt(new RunProperties(), 0);
            if (isBold)
            {
                runProps.AppendChild(new Bold());
            }

            cell.Append(para);
            return cell;
        }

        // Create a data row with alternating colors
        private TableRow CreateDataRow(string col1, string col2, string col3, string col4, bool altColor)
        {
            TableRow row = new TableRow();
            string bgColor = altColor ? "F2F2F2" : null;

            row.Append(
                CreateTableCell(col1, false, bgColor),
                CreateTableCell(col2, false, bgColor),
                CreateTableCell(col3, false, bgColor),
                CreateTableCell(col4, false, bgColor)
            );

            return row;
        }

        // Create a complex table with merged cells
        public void CreateComplexTable(string filePath)
        {
            using (WordprocessingDocument doc = WordprocessingDocument.Create(
                filePath, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = doc.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body body = mainPart.Document.AppendChild(new Body());

                // Add title
                Paragraph title = body.AppendChild(new Paragraph());
                Run titleRun = title.AppendChild(new Run(new Text("Employee Performance Review")));
                RunProperties titleProps = titleRun.InsertAt(new RunProperties(), 0);
                titleProps.AppendChild(new Bold());
                titleProps.AppendChild(new FontSize { Val = "28" });

                // Create table
                Table table = new Table();
                TableProperties tblProps = new TableProperties(
                    new TableBorders(
                        new TopBorder { Val = BorderValues.Single, Size = 12 },
                        new BottomBorder { Val = BorderValues.Single, Size = 12 },
                        new LeftBorder { Val = BorderValues.Single, Size = 12 },
                        new RightBorder { Val = BorderValues.Single, Size = 12 },
                        new InsideHorizontalBorder { Val = BorderValues.Single, Size = 6 },
                        new InsideVerticalBorder { Val = BorderValues.Single, Size = 6 }
                    )
                );
                table.AppendChild(tblProps);

                // Row 1: Merged header
                TableRow row1 = new TableRow();
                TableCell mergedCell = CreateTableCell("Q4 2024 Performance Metrics", true, "4472C4");

                // Merge across 3 columns
                TableCellProperties mergProps = mergedCell.GetFirstChild<TableCellProperties>();
                mergProps.Append(new GridSpan { Val = 3 });
                mergProps.Append(new VerticalMerge { Val = MergedCellValues.Restart });

                row1.Append(mergedCell);
                table.AppendChild(row1);

                // Row 2: Column headers
                TableRow row2 = new TableRow();
                row2.Append(
                    CreateTableCell("Employee", true, "B4C6E7"),
                    CreateTableCell("Score", true, "B4C6E7"),
                    CreateTableCell("Rating", true, "B4C6E7")
                );
                table.AppendChild(row2);

                // Data rows with conditional formatting
                table.AppendChild(CreateRatingRow("Alice Johnson", "95", "Excellent", "C6E0B4"));
                table.AppendChild(CreateRatingRow("Bob Smith", "87", "Very Good", "D9E1F2"));
                table.AppendChild(CreateRatingRow("Carol White", "78", "Good", "FFF2CC"));
                table.AppendChild(CreateRatingRow("David Brown", "65", "Needs Improvement", "F4B084"));

                body.AppendChild(table);
                mainPart.Document.Save();
            }
        }

        private TableRow CreateRatingRow(string name, string score, string rating, string bgColor)
        {
            TableRow row = new TableRow();
            row.Append(
                CreateTableCell(name, false, null),
                CreateTableCell(score, true, null),
                CreateTableCell(rating, false, bgColor)
            );
            return row;
        }

        //    private void CreateDocx(string filePath, string content)
        //{
        //    using (WordprocessingDocument doc = WordprocessingDocument.Create(
        //        filePath, DocumentFormat.OpenXml.WordprocessingDocumentType.Document))
        //    {
        //        MainDocumentPart mainPart = doc.AddMainDocumentPart();
        //        mainPart.Document = new Document();
        //        Body body = mainPart.Document.AppendChild(new Body());

        //        foreach (string line in content.Split('\n'))
        //        {
        //            Paragraph para = body.AppendChild(new Paragraph());
        //            Run run = para.AppendChild(new Run());
        //            run.AppendChild(new Text(line));
        //        }

        //        mainPart.Document.Save();
        //    }
        //}
    }
}
