import { Link, Outlet, useNavigate } from 'react-router-dom';

export default function AppLayout() {
  const navigate = useNavigate();
  const isLogged = Boolean(localStorage.getItem('token'));
  const email = localStorage.getItem('email');

  function logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('email');
    navigate('/');
  }

  return (
    <div className="page-shell">
      <header className="header">
        <Link className="brand" to="/">DataShare</Link>
        <nav>
          {isLogged ? (
            <>
              <Link to="/upload">Téléverser</Link>
              <Link to="/files">Mes fichiers</Link>
              <span className="user-mail">{email}</span>
              <button className="link-button" onClick={logout}>Déconnexion</button>
            </>
          ) : (
            <Link to="/login">Se connecter</Link>
          )}
        </nav>
      </header>
      <main>
        <Outlet />
      </main>
      <footer>Copyright DataShare© 2025</footer>
    </div>
  );
}
