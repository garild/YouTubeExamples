export interface Booking {
  id: string;
  destination: string;
  date: string;
  price: number;
}

export interface CreateBookingRequest {
  destination: string;
  date: string;
  price: number;
}

const API_BASE_URL = import.meta.env.VITE_BOOKING_API || 'http://localhost:5000/api'; // Or whatever port the .NET API runs on, we'll assume 5000 for local dev or 5057

export const getBookings = async (token: string): Promise<Booking[]> => {
  const response = await fetch(`${API_BASE_URL}/api/bookings`, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });

  if (!response.ok) {
    throw new Error('Failed to fetch bookings');
  }

  return response.json();
};

export const createBooking = async (
  token: string,
  request: CreateBookingRequest
): Promise<Booking> => {
  const response = await fetch(`${API_BASE_URL}/api/bookings`, {
    method: 'POST',
    headers: {
      Authorization: `Bearer ${token}`,
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(request),
  });

  if (!response.ok) {
    throw new Error('Failed to create booking');
  }

  return response.json();
};
