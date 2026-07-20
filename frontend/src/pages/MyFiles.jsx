import { useEffect, useState } from 'react';
import { deleteFile, getMyFiles } from '../api/client.js';

export default function MyFiles() {
  const [files, setFiles] = useState([]);
  const [filter, setFilter] = useState('all');
  const [error, setError] = useState('');

  async function loadFiles() {
    try {
      setFiles(await getMyFiles());
    } catch (err) {
      setError(err.message);
    }
  }

  useEffect(() => { loadFiles(); }, []);

  async function handleDelete(id) {
    if (!confirm('Supprimer ce fichier ? Cette action est irréversible.')) return;
    await deleteFile(id);
    loadFiles();
  }

  const visibleFiles = files.filter(file => {
    if (filter === 'active') return !file.isExpired;
    if (filter === 'expired') return file.isExpired;
    return true;
  });

  return (
    <section className="card list-card">
      <div className="list-header">
        <h1>Mes fichiers</h1>
        <div className="filters">
          <button className={filter === 'all' ? 'active' : ''} onClick={() => setFilter('all')}>Tous</button>
          <button className={filter === 'active' ? 'active' : ''} onClick={() => setFilter('active')}>Actifs</button>
          <button className={filter === 'expired' ? 'active' : ''} onClick={() => setFilter('expired')}>Expiré</button>
        </div>
      </div>
      {error && <p className="error">{error}</p>}
      {visibleFiles.length === 0 && <p>Aucun fichier à afficher pour le moment.</p>}
      {visibleFiles.map(file => (
        <article className="file-row" key={file.id}>
          <div>
            <strong>{file.originalName}</strong>
            <small>{file.isExpired ? 'Expiré' : `Expire le ${new Date(file.expiresAt).toLocaleDateString()}`}</small>
          </div>
          <div className="row-actions">
            {!file.isExpired && <a href={file.downloadUrl} target="_blank">Accéder</a>}
            <button onClick={() => handleDelete(file.id)}>Supprimer</button>
          </div>
        </article>
      ))}
    </section>
  );
}
