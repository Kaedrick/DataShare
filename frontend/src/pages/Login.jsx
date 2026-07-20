import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { login } from '../api/client.js';

export default function Login() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const navigate = useNavigate();

  async function handleSubmit(event) {
    event.preventDefault();
    setError('');
    try {
      const data = await login(email, password);
      localStorage.setItem('token', data.token);
      localStorage.setItem('email', data.email);
      navigate('/files');
    } catch (err) {
      setError(err.message);
    }
  }

  return (
    <form className="card form" onSubmit={handleSubmit} data-cy="login-form">
      <h1>Connexion</h1>
      {error && <p className="error">{error}</p>}
      <label>Email</label>
      <input data-cy="login-email" value={email} onChange={e => setEmail(e.target.value)} placeholder="Saisissez votre email..." type="email" required />
      <label>Mot de passe</label>
      <input data-cy="login-password" value={password} onChange={e => setPassword(e.target.value)} placeholder="Saisissez votre mot de passe..." type="password" required />
      <button className="primary-button" data-cy="login-submit">Connexion</button>
      <Link to="/register">Créer un compte</Link>
    </form>
  );
}
