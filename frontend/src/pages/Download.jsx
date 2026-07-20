import { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { downloadFile, getDownloadMetadata } from '../api/client.js';

export default function Download() {
  const { token } = useParams();
  const [metadata, setMetadata] = useState(null);
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');

  useEffect(() => {
    getDownloadMetadata(token).then(setMetadata).catch(err => setError(err.message));
  }, [token]);

  async function handleDownload() {
    setError('');
    try {
      const blob = await downloadFile(token, password);
      const url = URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      link.download = metadata.originalName;
      link.click();
      URL.revokeObjectURL(url);
    } catch (err) {
      setError(err.message);
    }
  }

  if (error && !metadata) return <section className="card"><h1>Télécharger un fichier</h1><p className="error">{error}</p></section>;
  if (!metadata) return <section className="card"><p>Chargement...</p></section>;

  return (
    <section className="card download-card">
      <h1>Télécharger un fichier</h1>
      {metadata.isExpired ? (
        <p>Ce fichier n'est plus disponible en téléchargement car il a expiré.</p>
      ) : (
        <>
          <h2>{metadata.originalName}</h2>
          <p>{(metadata.size / 1024 / 1024).toFixed(1)} Mo</p>
          <p>Ce fichier expirera le {new Date(metadata.expiresAt).toLocaleDateString()}.</p>
          {metadata.requiresPassword && (
            <>
              <label>Mot de passe</label>
              <input type="password" value={password} onChange={e => setPassword(e.target.value)} placeholder="Saisissez le mot de passe..." />
            </>
          )}
          {error && <p className="error">{error}</p>}
          <button className="primary-button" data-cy="download-submit" onClick={handleDownload}>Télécharger</button>
        </>
      )}
    </section>
  );
}
