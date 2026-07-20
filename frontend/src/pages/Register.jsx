import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { register } from '../api/client.js';

export default function Register() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirm, setConfirm] = useState('');
  const [error, setError] = useState('');
  const navigate = useNavigate();

  async function handleSubmit(event) {
    event.preventDefault();
    setError('');
    if (password.length < 8) return setError('Le mot de passe doit contenir au moins 8 caractères.');
    if (password !== confirm) return setError('Les mots de passe ne correspondent pas.');

    try {
      const data = await register(email, password);
      localStorage.setItem('token', data.token);
      localStorage.setItem('email', data.email);
      navigate('/upload');
    } catch (err) {
      setError(err.message);
    }
  }

  return (
    <form className="card form" onSubmit={handleSubmit} data-cy="register-form">
      <h1>Créer un compte</h1>
      {error && <p className="error">{error}</p>}
      <label>Email</label>
      <input data-cy="register-email" value={email} onChange={e => setEmail(e.target.value)} placeholder="Saisissez votre email..." type="email" required />
      <label>Mot de passe</label>
      <input data-cy="register-password" value={password} onChange={e => setPassword(e.target.value)} placeholder="Saisissez votre mot de passe..." type="password" required />
      <label>Vérification du mot de passe</label>
      <input data-cy="register-confirm" value={confirm} onChange={e => setConfirm(e.target.value)} placeholder="Saisissez le à nouveau" type="password" required />
      <button className="primary-button" data-cy="register-submit">Créer mon compte</button>
      <Link to="/login">J'ai déjà un compte</Link>
    </form>
  );
}
