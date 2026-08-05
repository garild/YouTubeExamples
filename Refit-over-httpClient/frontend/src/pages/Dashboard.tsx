import React, { useEffect, useState } from 'react';
import { useAuth0 } from '@auth0/auth0-react';
import { User, Mail, Calendar, MapPin, DollarSign, Plus, Loader } from 'lucide-react';
import { getBookings, createBooking, type Booking } from '../services/api';

const Dashboard: React.FC = () => {
  const { user, getAccessTokenSilently } = useAuth0();
  const [bookings, setBookings] = useState<Booking[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Form state
  const [destination, setDestination] = useState('');
  const [date, setDate] = useState('');
  const [price, setPrice] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    const fetchBookings = async () => {
      try {
        const token = await getAccessTokenSilently();
        const data = await getBookings(token);
        setBookings(data);
      } catch (err) {
        console.error(err);
        setError('Failed to load bookings.');
      } finally {
        setIsLoading(false);
      }
    };

    if (user) {
      fetchBookings();
    }
  }, [user, getAccessTokenSilently]);

  const handleCreateBooking = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsSubmitting(true);
    
    try {
      const token = await getAccessTokenSilently();
      const newBooking = await createBooking(token, {
        destination,
        date: new Date(date).toISOString(),
        price: parseFloat(price)
      });
      
      setBookings([...bookings, newBooking]);
      
      // Reset form
      setDestination('');
      setDate('');
      setPrice('');
    } catch (err) {
      console.error(err);
      setError('Failed to create booking.');
    } finally {
      setIsSubmitting(false);
    }
  };

  if (!user) {
    return null; 
  }

  return (
    <div className="dashboard-container slide-up" style={{ maxWidth: '1000px', margin: '0 auto', padding: '2rem' }}>
      <div className="dashboard-header" style={{ marginBottom: '2rem' }}>
        <h2 className="dashboard-title" style={{ fontSize: '2.5rem', fontWeight: 800 }}>Dashboard</h2>
        <p className="dashboard-subtitle text-muted">Welcome back to your personalized space.</p>
      </div>
      
      <div className="dashboard-grid" style={{ display: 'grid', gridTemplateColumns: '1fr 2fr', gap: '2rem' }}>
        {/* Profile Column */}
        <div className="profile-column">
          <div className="profile-card glass-panel" style={{ padding: '2rem', borderRadius: '16px', background: 'rgba(255,255,255,0.05)', border: '1px solid rgba(255,255,255,0.1)' }}>
            <div className="profile-image-container" style={{ marginBottom: '1.5rem', display: 'flex', justifyContent: 'center' }}>
              {user.picture ? (
                <img src={user.picture} alt={user.name} className="profile-image" style={{ width: '100px', height: '100px', borderRadius: '50%', objectFit: 'cover' }} />
              ) : (
                <div className="profile-image-placeholder" style={{ width: '100px', height: '100px', borderRadius: '50%', background: '#333', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                  <User size={40} />
                </div>
              )}
            </div>
            
            <div className="profile-details" style={{ textAlign: 'center' }}>
              <h3 className="profile-name" style={{ fontSize: '1.5rem', margin: '0 0 0.5rem' }}>{user.name}</h3>
              
              <div className="profile-info-row" style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', gap: '0.5rem', marginBottom: '1rem', color: '#888' }}>
                <Mail size={16} />
                <span className="profile-email">{user.email}</span>
              </div>
              
              <div className="badge-container" style={{ display: 'flex', gap: '0.5rem', justifyContent: 'center', flexWrap: 'wrap' }}>
                {user.email_verified && (
                  <span className="badge badge-success" style={{ background: 'rgba(46, 204, 113, 0.2)', color: '#2ecc71', padding: '0.25rem 0.75rem', borderRadius: '999px', fontSize: '0.8rem' }}>Verified Account</span>
                )}
                <span className="badge badge-primary" style={{ background: 'rgba(52, 152, 219, 0.2)', color: '#3498db', padding: '0.25rem 0.75rem', borderRadius: '999px', fontSize: '0.8rem' }}>Standard User</span>
              </div>
            </div>
          </div>
          
          <div className="card glass-panel" style={{ marginTop: '2rem', padding: '2rem', borderRadius: '16px', background: 'rgba(255,255,255,0.05)', border: '1px solid rgba(255,255,255,0.1)' }}>
            <h3 style={{ margin: '0 0 1.5rem', display: 'flex', alignItems: 'center', gap: '0.5rem' }}><Plus size={20}/> New Booking</h3>
            <form onSubmit={handleCreateBooking} style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
              <div>
                <label style={{ display: 'block', marginBottom: '0.5rem', fontSize: '0.9rem', color: '#ccc' }}>Destination</label>
                <input 
                  type="text" 
                  value={destination} 
                  onChange={e => setDestination(e.target.value)} 
                  required
                  style={{ width: '100%', padding: '0.75rem', borderRadius: '8px', border: '1px solid #444', background: '#222', color: 'white' }} 
                  placeholder="e.g. Paris"
                />
              </div>
              <div>
                <label style={{ display: 'block', marginBottom: '0.5rem', fontSize: '0.9rem', color: '#ccc' }}>Date</label>
                <input 
                  type="date" 
                  value={date} 
                  onChange={e => setDate(e.target.value)} 
                  required
                  style={{ width: '100%', padding: '0.75rem', borderRadius: '8px', border: '1px solid #444', background: '#222', color: 'white' }} 
                />
              </div>
              <div>
                <label style={{ display: 'block', marginBottom: '0.5rem', fontSize: '0.9rem', color: '#ccc' }}>Price ($)</label>
                <input 
                  type="number" 
                  step="0.01" 
                  value={price} 
                  onChange={e => setPrice(e.target.value)} 
                  required
                  style={{ width: '100%', padding: '0.75rem', borderRadius: '8px', border: '1px solid #444', background: '#222', color: 'white' }} 
                  placeholder="1250.00"
                />
              </div>
              <button 
                type="submit" 
                disabled={isSubmitting}
                style={{ 
                  marginTop: '1rem', 
                  padding: '0.75rem', 
                  borderRadius: '8px', 
                  border: 'none', 
                  background: 'linear-gradient(135deg, #6e8efb, #a777e3)', 
                  color: 'white', 
                  fontWeight: 'bold',
                  cursor: isSubmitting ? 'not-allowed' : 'pointer',
                  opacity: isSubmitting ? 0.7 : 1
                }}>
                {isSubmitting ? 'Booking...' : 'Create Booking'}
              </button>
            </form>
          </div>
        </div>

        {/* Content Column */}
        <div className="content-column">
          <div className="card glass-panel" style={{ padding: '2rem', borderRadius: '16px', background: 'rgba(255,255,255,0.05)', border: '1px solid rgba(255,255,255,0.1)', height: '100%' }}>
            <h3 style={{ margin: '0 0 1.5rem', fontSize: '1.5rem' }}>Your Bookings</h3>
            
            {error && (
              <div style={{ padding: '1rem', background: 'rgba(231, 76, 60, 0.2)', color: '#e74c3c', borderRadius: '8px', marginBottom: '1.5rem' }}>
                {error}
              </div>
            )}
            
            {isLoading ? (
              <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '200px', color: '#888' }}>
                <Loader className="spin" size={32} />
              </div>
            ) : bookings.length === 0 ? (
              <div style={{ textAlign: 'center', padding: '3rem 1rem', color: '#888' }}>
                <Calendar size={48} style={{ margin: '0 auto 1rem', opacity: 0.5 }} />
                <p>No bookings found. Create one to get started!</p>
              </div>
            ) : (
              <div className="bookings-list" style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
                {bookings.map(booking => (
                  <div key={booking.id} className="booking-item" style={{ 
                    display: 'flex', 
                    justifyContent: 'space-between', 
                    alignItems: 'center',
                    padding: '1.25rem',
                    background: 'rgba(0,0,0,0.2)',
                    borderRadius: '12px',
                    border: '1px solid rgba(255,255,255,0.05)',
                    transition: 'transform 0.2s, background 0.2s'
                  }}>
                    <div>
                      <h4 style={{ margin: '0 0 0.5rem', fontSize: '1.2rem', display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
                        <MapPin size={18} color="#a777e3" /> 
                        {booking.destination}
                      </h4>
                      <p style={{ margin: 0, color: '#aaa', display: 'flex', alignItems: 'center', gap: '0.5rem', fontSize: '0.9rem' }}>
                        <Calendar size={14} />
                        {new Date(booking.date).toLocaleDateString(undefined, { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' })}
                      </p>
                    </div>
                    <div style={{ display: 'flex', alignItems: 'center', fontSize: '1.25rem', fontWeight: 'bold', color: '#6e8efb' }}>
                      <DollarSign size={20} />
                      {booking.price.toFixed(2)}
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
};

export default Dashboard;
