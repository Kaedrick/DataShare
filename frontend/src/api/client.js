const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';

function authHeaders() {
  const token = localStorage.getItem('token');
  return token ? { Authorization: `Bearer ${token}` } : {};
}

async function readResponse(response) {
  if (!response.ok) {
    let message = 'Erreur serveur';
    try {
      const data = await response.json();
      message = data.message || message;
    } catch (_) {}
    throw new Error(message);
  }

  if (response.status === 204) return null;
  return response.json();
}

export async function register(email, password) {
  const response = await fetch(`${API_URL}/auth/register`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email, password })
  });
  return readResponse(response);
}

export async function login(email, password) {
  const response = await fetch(`${API_URL}/auth/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email, password })
  });
  return readResponse(response);
}

export async function uploadFile(file, expirationDays, password) {
  const form = new FormData();
  form.append('file', file);
  form.append('expirationDays', expirationDays);
  if (password) form.append('password', password);

  const response = await fetch(`${API_URL}/files/upload`, {
    method: 'POST',
    headers: authHeaders(),
    body: form
  });
  return readResponse(response);
}

export async function getMyFiles() {
  const response = await fetch(`${API_URL}/files/me`, { headers: authHeaders() });
  return readResponse(response);
}

export async function deleteFile(id) {
  const response = await fetch(`${API_URL}/files/${id}`, {
    method: 'DELETE',
    headers: authHeaders()
  });
  return readResponse(response);
}

export async function getDownloadMetadata(token) {
  const response = await fetch(`${API_URL}/download/${token}`);
  return readResponse(response);
}

export async function downloadFile(token, password) {
  const response = await fetch(`${API_URL}/download/${token}`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ password })
  });

  if (!response.ok) {
    const data = await response.json().catch(() => ({}));
    throw new Error(data.message || 'Téléchargement impossible');
  }

  return response.blob();
}
