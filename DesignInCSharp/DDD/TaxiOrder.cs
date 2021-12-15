using System;
using System.Globalization;
using System.Linq;
using Ddd.Infrastructure;

namespace Ddd.Taxi.Domain
{
    // In real aplication it whould be the place where database is used to find driver by its Id.
    // But in this exercise it is just a mock to simulate database
    public class DriversRepository
    {
        public void FillDriverToOrder(int driverId, TaxiOrder order)
        {
            if (driverId == 15)
                order.SetData(new Driver(
                        driverId,
                        new PersonName("Drive", "Driverson")),
                    "Lada sedan",
                    "Baklazhan",
                    "A123BT 66");
            else
                throw new Exception("Unknown driver id " + driverId);
        }
    }

    public class TaxiApi : ITaxiApi<TaxiOrder>
    {

        private readonly DriversRepository driversRepo;

        private readonly Func<DateTime> currentTime;
        private int idCounter;

        public TaxiApi(DriversRepository driversRepo, Func<DateTime> currentTime)
        {
            this.driversRepo = driversRepo;
            this.currentTime = currentTime;
        }

        public TaxiOrder CreateOrderWithoutDestination(string firstName, string lastName, string street, string building) =>
            new TaxiOrder
            (
                idCounter++,
                new PersonName(firstName, lastName),
                new Address(street, building),
                currentTime
            );

        public void UpdateDestination(TaxiOrder order, string street, string building) => 
            order.UpdateDestination(new Address(street, building));

        public void AssignDriver(TaxiOrder order, int driverId)
        {
            order.AssignDriver();
            driversRepo.FillDriverToOrder(driverId, order);
        }

        public void UnassignDriver(TaxiOrder order) => 
            order.UnassignDriver();

        public string GetDriverFullInfo(TaxiOrder order) => 
            order.GetStatus == TaxiOrderStatus.WaitingForDriver ? null : order.GetDriverFullInfo();

        public string GetShortOrderInfo(TaxiOrder order) => order.GetShortOrderInfo();

        private DateTime GetLastProgressTime(TaxiOrder order) => order.GetLastProgressTime();

        private string FormatName(string firstName, string lastName) => 
            string.Join(" ", new[] { firstName, lastName }.Where(n => n != null));

        private string FormatAddress(string street, string building) => 
            string.Join(" ", new[] { street, building }.Where(n => n != null));

        public void Cancel(TaxiOrder order) => order.Cancel();

        public void StartRide(TaxiOrder order) => order.StartRide();

        public void FinishRide(TaxiOrder order) => order.FinishRide();
    }

    public class Driver : Entity<int>
    {
        public int Id;
        public PersonName Name;

        public Car Car { get; set; }

        public Driver(int pId, PersonName pName) : base(pId)
        {
            Id = pId;
            Name = pName;
        }
    }

    public class Car : ValueType<Car>
    {
        public string CarColor;
        public string CarModel;
        public string CarPlateNumber;

        public Car(string pCarColor, string pCarModel, string pCarPlateNumber)
        {
            CarColor = pCarColor;
            CarModel = pCarModel;
            CarPlateNumber = pCarPlateNumber;
        }
    }

    public class TaxiOrder : Entity<int>
    {
        private int id;
        private PersonName clientName;
        private Address start;
        private Address destination;
        private Driver driver;
        private DateTime creationTime;
        private DateTime driverAssignmentTime;
        private DateTime cancelTime;
        private DateTime sStartRideTime;
        private DateTime finishRideTime;
        private Func<DateTime> currentTime;
        
        public PersonName ClientName { get; }
        public Address Start { get; }
        public Address Destination { get; }
        public Driver Driver { get; }

        public TaxiOrderStatus GetStatus { get; private set; }

        public void SetData(Driver pDriver, string pCarModel, string pCarColor, string pCarPlateNumber)
        {
            driver = pDriver;
            driver.Car = new Car(pCarColor, pCarModel, pCarPlateNumber);
        }

        public TaxiOrder(int pId, PersonName pClientName, Address pStart, Func<DateTime> pCurrentTime) : base(pId)
        {
            id = pId;
            clientName = pClientName;
            start = pStart;
            creationTime = pCurrentTime();
            currentTime = pCurrentTime;
        }

        public void UpdateDestination(Address p) => destination = p;

        public void AssignDriver()
        {
            if (driver != null)
                throw new InvalidOperationException("Blablabla");

            driverAssignmentTime = currentTime();
            GetStatus = TaxiOrderStatus.WaitingCarArrival;
        }

        public void UnassignDriver()
        {
            if (GetStatus == TaxiOrderStatus.InProgress || GetStatus == TaxiOrderStatus.Finished)
                throw new System.InvalidOperationException("Нельзя снять водителя с заказа после начала поездки");

            if (driver == null)
                throw new System.InvalidOperationException("WaitingForDriver");
            
            driver = null;
            GetStatus = TaxiOrderStatus.WaitingForDriver;
        }

        public string GetDriverFullInfo() =>
            GetStatus == TaxiOrderStatus.WaitingForDriver
                ? null
                : string.Join(" ",
                    "Id: " + driver.Id,
                    "DriverName: " + FormatName((driver == null) ? null : driver.Name),
                    "Color: " + driver.Car.CarColor,
                    "CarModel: " + driver.Car.CarModel,
                    "PlateNumber: " + driver.Car.CarPlateNumber);

        public string GetShortOrderInfo() =>
            string.Join(" ",
                "OrderId: " + id,
                "Status: " + GetStatus,
                "Client: " + FormatName(clientName),
                "Driver: " + FormatName((driver == null) ? null : driver.Name),
                "From: " + FormatAddress(start),
                "To: " + FormatAddress(destination),
                "LastProgressTime: " + GetLastProgressTime().ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));

        public DateTime GetLastProgressTime()
        {
            switch (GetStatus)
            {
                case TaxiOrderStatus.WaitingForDriver:
                    return creationTime;
                case TaxiOrderStatus.WaitingCarArrival:
                    return driverAssignmentTime;
                case TaxiOrderStatus.InProgress:
                    return sStartRideTime;
                case TaxiOrderStatus.Finished:
                    return finishRideTime;
                case TaxiOrderStatus.Canceled:
                    return cancelTime;
                default:
                    throw new NotSupportedException(GetStatus.ToString());
            }
        }

        private string FormatName(PersonName p) => 
            (p == null) ? "" : p.FirstName + " " + p.LastName;

        private string FormatAddress(Address p) => 
            (p == null) ? "" : p.Street + " " + p.Building;

        public void Cancel()
        {
            if (GetStatus == TaxiOrderStatus.InProgress || GetStatus == TaxiOrderStatus.Finished)
                throw new InvalidOperationException("Нельзя отменить заказ после начала поездки");

            GetStatus = TaxiOrderStatus.Canceled;
            cancelTime = currentTime();
        }

        public void StartRide()
        {
            if (driver == null)
                throw new InvalidOperationException("Нельзя начать поездку, если у тебя нет водителя (мем бы сюда)");

            GetStatus = TaxiOrderStatus.InProgress;
            sStartRideTime = currentTime();
        }

        public void FinishRide()
        {
            if (driver == null)
                throw new InvalidOperationException(
                    "Нельзя завершить поездку, если у тебя нет водителя (и сюда тоже)");

            if (GetStatus != TaxiOrderStatus.InProgress)
                throw new InvalidOperationException("Нельзя завершить поездку до ее начала");

            GetStatus = TaxiOrderStatus.Finished;
            finishRideTime = currentTime();
        }
    }
}
