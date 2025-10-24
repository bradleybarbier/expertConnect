# expertConnect - Modern Booking API

A sophisticated appointment booking system showcasing modern .NET development practices, clean architecture, and real-world business logic handling.

## Quick Start 🚀

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- A modern web browser

### Running the API

```bash
cd backend/src/Api
dotnet watch run --launch-profile development
```

The API will automatically open in your browser at http://localhost:5000/swagger

### Testing the API
Once the Swagger UI opens, you can test the API with this workflow:

1. Create an Expert:
   - Use POST /api/experts
   - Sample payload:
   ```json
   {
     "name": "Dr. Jane Smith",
     "specialization": "Career Counseling",
     "email": "jane.smith@example.com",
     "bio": "15 years of experience in career guidance",
     "hourlyRate": 150
   }
   ```

2. Get Available Time Slots:
   - Copy the expert's ID from the response above
   - Use GET /api/experts/{expertId}/available-slots
   - Set the date parameter (e.g., "2024-10-24")

3. Create a Booking:
   - Use POST /api/bookings
   - Sample payload:
   ```json
   {
     "expertId": "paste-expert-id-here",
     "clientName": "John Doe",
     "clientEmail": "john.doe@example.com",
     "startTime": "2024-10-24T10:00:00Z",
     "notes": "Initial career consultation"
   }
   ```

4. View Booking:
   - Copy the booking ID from the response above
   - Use GET /api/bookings/{id}

## Technical Highlights
- .NET 8 Minimal API
- Entity Framework Core with SQLite
- Clean Architecture
- Swagger/OpenAPI documentation
- Proper error handling and validation
- Transaction management
- Time slot management with double-booking prevention

## Project Structure
```
/expertConnect/
├── backend/
│   ├── src/
│   │   ├── Api/           # API endpoints and services
│   │   ├── Core/          # Domain models
│   │   └── Infrastructure/# Data access and external services
│   └── tests/            # (Coming soon)
└── frontend/            # (Phase 3)