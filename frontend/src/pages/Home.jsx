import { Link } from 'react-router-dom';

export default function Home() {
  const isLogged = Boolean(localStorage.getItem('token'));
  return (
    <section className="hero card">
      <p className="eyebrow">Transfert sécurisé</p>
      <h1>Tu veux partager un fichier ?</h1>
      <p>Dépose un fichier, choisis sa durée de validité et partage un lien temporaire.</p>
      <Link className="primary-button" to={isLogged ? '/upload' : '/login'}>
        {isLogged ? 'Ajouter un fichier' : 'Se connecter'}
      </Link>
    </section>
  );
}
