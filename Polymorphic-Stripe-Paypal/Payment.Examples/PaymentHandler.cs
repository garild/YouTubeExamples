namespace Payment.Examples
{
    public class PaymentHandler
    {
        public class PaymentService
        {
            private readonly PaymentGateway _gateway;
            private readonly EmailService _emailService;
            private readonly PaymentRepository _repository;

            public PaymentService(
                PaymentGateway gateway,
                EmailService emailService,
                PaymentRepository repository)
            {
                _gateway = gateway;
                _emailService = emailService;
                _repository = repository;
            }

            public void ProcessPayment(Guid bookingId, decimal amount)
            {
                var payment = new Payment(
                    bookingId,
                    amount);

                _gateway.Charge(payment);

                _repository.Save(payment);

                _emailService.Send(
                    "garib@example.com",
                    "Payment successful!");
            }
        }

        // Entity
        public class Payment
        {
            public Guid BookingId { get; }

            public decimal Amount { get; }

            public Payment(
                Guid bookingId,
                decimal amount)
            {
                BookingId = bookingId;
                Amount = amount;
            }
        }

        public class PaymentGateway
        {
            public void Charge( )
            {
                Console.WriteLine(
                    $"Charging £{payment.Amount}");
            }
        }

        public class PaymentRepository
        {
            public void Save(Payment payment)
            {
                Console.WriteLine(
                    "Saving payment to DB");
            }
        }

        public class EmailService
        {
            public void Send(
                string email,
                string message)
            {
                Console.WriteLine(
                    $"Sending email to {email}");
            }
        }
    }
}
