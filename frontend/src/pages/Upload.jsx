import { useState } from 'react';
import { uploadFile } from '../api/client.js';

export default function Upload() {
  const [file, setFile] = useState(null);
  const [expirationDays, setExpirationDays] = useState(7);
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [result, setResult] = useState(null);

  async function handleSubmit(event) {
    event.preventDefault();
    setError('');
    setResult(null);

    if (!file) return setError('Ajoutez un fichier avant de téléverser.');
    if (file.size > 1024 * 1024 * 1024) return setError('La taille des fichiers est limitée à 1 Go.');
    if (password && password.length < 6) return setError('Le mot de passe doit contenir au moins 6 caractères.');

    try {
      setResult(await uploadFile(file, expirationDays, password));
    } catch (err) {
      setError(err.message);
    }
  }

  return (
    <section className="card upload-card">
      <h1>Ajouter un fichier</h1>
      <form onSubmit={handleSubmit} className="form">
        <label className="dropzone">
          <input data-cy="upload-file" type="file" onChange={e => setFile(e.target.files[0])} />
          {file ? (
            <span>{file.name}<small>{(file.size / 1024 / 1024).toFixed(1)} Mo</small></span>
          ) : (
            <span>Choisir un fichier<small>Taille maximum : 1 Go</small></span>
          )}
        </label>

        <label>Mot de passe <small>Optionnel</small></label>
        <input data-cy="upload-password" value={password} onChange={e => setPassword(e.target.value)} type="password" placeholder="Optionnel" />

        <label>Expiration</label>
        <select data-cy="upload-expiration" value={expirationDays} onChange={e => setExpirationDays(Number(e.target.value))}>
          <option value="1">Une journée</option>
          <option value="3">Trois jours</option>
          <option value="7">Une semaine</option>
        </select>

        {error && <p className="error">{error}</p>}
        <button className="primary-button" data-cy="upload-submit">Téléverser</button>
      </form>

      {result && (
        <div className="success-box">
          <strong>Félicitations, ton fichier sera conservé chez nous jusqu'au {new Date(result.expiresAt).toLocaleDateString()} !</strong>
          <input data-cy="download-url" readOnly value={result.downloadUrl} />
          <button onClick={() => navigator.clipboard.writeText(result.downloadUrl)}>Copier le lien</button>
        </div>
      )}
    </section>
  );
}
