using AppointmentBooking;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppointmentBooking.Tests;

[TestClass]
public class AppointmentBookingServiceTests
{
    [TestMethod]
    public void BookAppointment_WhenDoctorHasAvailableSlots_ReturnsSuccess()
    {
        var doctor = new Doctor("D001", "Dr Mark", 2);
        var patient = new Patient("P001", "Diana William");
        var request = new AppointmentRequest(patient, doctor, DateTime.Today.AddDays(1));
        var service = new AppointmentBookingService();

        BookingResult result = service.BookAppointment(request);

        Assert.IsTrue(result.Success);
    }

    [TestMethod]
    public void BookAppointment_WhenDoctorHasNoAvailableSlots_ReturnsFailure()
    {
        var doctor = new Doctor("D001", "Dr Mark", 0);
        var patient = new Patient("P001", "Diana William");
        var request = new AppointmentRequest(patient, doctor, DateTime.Today.AddDays(1));
        var service = new AppointmentBookingService();

        BookingResult result = service.BookAppointment(request);

        Assert.IsFalse(result.Success);
    }

    [TestMethod]
    public void BookAppointment_WhenSuccessful_DecreasesAvailableSlots()
    {
        var doctor = new Doctor("D001", "Dr Mark", 2);
        var patient = new Patient("P001", "Diana William");
        var request = new AppointmentRequest(patient, doctor, DateTime.Today.AddDays(1));
        var service = new AppointmentBookingService();

        service.BookAppointment(request);

        Assert.AreEqual(1, doctor.AvailableSlots);
    }

    [TestMethod]
    public void BookAppointment_WhenFailed_DoesNotDecreaseAvailableSlots()
    {
        var doctor = new Doctor("D001", "Dr Mark", 0);
        var patient = new Patient("P001", "Diana William");
        var request = new AppointmentRequest(patient, doctor, DateTime.Today.AddDays(1));
        var service = new AppointmentBookingService();

        service.BookAppointment(request);

        Assert.AreEqual(0, doctor.AvailableSlots);
    }

    [TestMethod]
    public void Doctor_WhenAvailableSlotsIsNegative_ThrowsException()
    {
        try
        {
            var doctor = new Doctor("D001", "Dr Mark", -1);
            Assert.Fail("Expected ArgumentException was not thrown.");
        }
        catch (ArgumentException)
        {
        }
    }

    [TestMethod]
    public void Patient_WhenIdIsEmpty_ThrowsException()
    {
        try
        {
            var patient = new Patient("", "Diana William");
            Assert.Fail("Expected ArgumentException was not thrown.");
        }
        catch (ArgumentException)
        {
        }
    }

    [TestMethod]
    public void Patient_WhenLegalNameIsEmpty_ThrowsException()
    {
        try
        {
            var patient = new Patient("P001", "");
            Assert.Fail("Expected ArgumentException was not thrown.");
        }
        catch (ArgumentException)
        {
        }
    }

    [TestMethod]
    public void Patient_WhenPreferredNameExists_DisplayNameUsesPreferredName()
    {
        var patient = new Patient("P001", "Diana William", "Aroha");

        Assert.AreEqual("Aroha", patient.DisplayName);
    }

    [TestMethod]
    public void Patient_WhenPreferredNameMissing_DisplayNameUsesLegalName()
    {
        var patient = new Patient("P001", "Diana William");

        Assert.AreEqual("Diana William", patient.DisplayName);
    }

    [TestMethod]
    public void AppointmentRequest_WhenRequestedDateIsPast_ThrowsException()
    {
        var doctor = new Doctor("D001", "Dr Mark", 2);
        var patient = new Patient("P001", "Diana William");

        try
        {
            var request = new AppointmentRequest(patient, doctor, DateTime.Today.AddDays(-1));
            Assert.Fail("Expected ArgumentException was not thrown.");
        }
        catch (ArgumentException)
        {
        }
    }

    [TestMethod]
    public void AppointmentRequest_WhenPatientIsNull_ThrowsException()
    {
        var doctor = new Doctor("D001", "Dr Mark", 2);

        try
        {
            var request = new AppointmentRequest(null, doctor, DateTime.Today.AddDays(1));
            Assert.Fail("Expected ArgumentNullException was not thrown.");
        }
        catch (ArgumentNullException)
        {
        }
    }

    [TestMethod]
    public void AppointmentRequest_WhenDoctorIsNull_ThrowsException()
    {
        var patient = new Patient("P001", "Diana William");

        try
        {
            var request = new AppointmentRequest(patient, null, DateTime.Today.AddDays(1));
            Assert.Fail("Expected ArgumentNullException was not thrown.");
        }
        catch (ArgumentNullException)
        {
        }
    }

    [TestMethod]
    public void BookAppointment_WhenSuccessful_ReturnsHelpfulMessage()
    {
        var doctor = new Doctor("D001", "Dr Mark", 2);
        var patient = new Patient("P001", "Diana William", "Aroha");
        var request = new AppointmentRequest(patient, doctor, DateTime.Today.AddDays(1));
        var service = new AppointmentBookingService();

        BookingResult result = service.BookAppointment(request);

        StringAssert.Contains(result.Message, "Appointment booked successfully");
        StringAssert.Contains(result.Message, "Aroha");
    }

    [TestMethod]
    public void BookAppointment_WhenNoSlots_ReturnsHelpfulMessage()
    {
        var doctor = new Doctor("D001", "Dr Mark", 0);
        var patient = new Patient("P001", "Diana William");
        var request = new AppointmentRequest(patient, doctor, DateTime.Today.AddDays(1));
        var service = new AppointmentBookingService();

        BookingResult result = service.BookAppointment(request);

        StringAssert.Contains(result.Message, "no available slots");
    }

    [TestMethod]
    public void BookAppointment_WhenRequestIsNull_ReturnsFailureMessage()
    {
        var service = new AppointmentBookingService();

        BookingResult result = service.BookAppointment(null);

        Assert.IsFalse(result.Success);
        StringAssert.Contains(result.Message, "missing");
    }

    // ========================================
    // EXTRA TESTS FOR TASK 2
    // ========================================

    [TestMethod]
    public void BookAppointment_WhenDateIsToday_ReturnsFailureWithClearMessage()
    {
        var doctor = new Doctor("D001", "Dr Mark", 5);
        var patient = new Patient("P001", "Diana William");
        var request = new AppointmentRequest(patient, doctor, DateTime.Today);
        var service = new AppointmentBookingService();

        BookingResult result = service.BookAppointment(request);

        Assert.IsFalse(result.Success);
        StringAssert.Contains(result.Message, "at least one day in advance");
    }

    [TestMethod]
    public void BookAppointment_WhenDateIsTomorrow_ReturnsSuccess()
    {
        var doctor = new Doctor("D001", "Dr Mark", 5);
        var patient = new Patient("P001", "Diana William");
        var request = new AppointmentRequest(patient, doctor, DateTime.Today.AddDays(1));
        var service = new AppointmentBookingService();

        BookingResult result = service.BookAppointment(request);

        Assert.IsTrue(result.Success);
    }

    [TestMethod]
    public void BookAppointment_WhenPatientIdIsMissing_ThrowsException()
    {
        // Patient constructor already validates that ID cannot be empty
        try
        {
            var patient = new Patient("", "Diana William");
            Assert.Fail("Expected ArgumentException was not thrown.");
        }
        catch (ArgumentException)
        {
            // Expected - test passes
        }
    }

    [TestMethod]
    public void BookAppointment_WhenSuccessful_MessageIncludesDoctorsName()
    {
        var doctor = new Doctor("D001", "Dr Mark", 5);
        var patient = new Patient("P001", "Diana William");
        var request = new AppointmentRequest(patient, doctor, DateTime.Today.AddDays(1));
        var service = new AppointmentBookingService();

        BookingResult result = service.BookAppointment(request);

        StringAssert.Contains(result.Message, "Dr Mark");
    }

    [TestMethod]
    public void BookAppointment_WhenSuccessful_MessageIncludesPatientDisplayName()
    {
        var doctor = new Doctor("D001", "Dr Mark", 5);
        var patient = new Patient("P001", "Diana William", "Aroha");
        var request = new AppointmentRequest(patient, doctor, DateTime.Today.AddDays(1));
        var service = new AppointmentBookingService();

        BookingResult result = service.BookAppointment(request);

        StringAssert.Contains(result.Message, "Aroha");
    }
}