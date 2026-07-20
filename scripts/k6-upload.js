import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
  vus: 1,
  iterations: 5,
  thresholds: {
    http_req_failed: ['rate<0.05'],
    http_req_duration: ['p(95)<1500']
  }
};

const apiUrl = __ENV.API_URL || 'http://localhost:5000/api';

export default function () {
  const email = `perf.${Date.now()}.${Math.random()}@example.com`;
  const password = 'Password123!';

  const register = http.post(`${apiUrl}/auth/register`, JSON.stringify({ email, password }), {
    headers: { 'Content-Type': 'application/json' }
  });

  check(register, {
    'register status 200': response => response.status === 200,
    'register has token': response => Boolean(response.json('token'))
  });

  const token = register.json('token');
  const payload = {
    file: http.file('contenu performance', 'perf.txt', 'text/plain'),
    expirationDays: '7'
  };

  const upload = http.post(`${apiUrl}/files/upload`, payload, {
    headers: { Authorization: `Bearer ${token}` }
  });

  check(upload, {
    'upload status 200': response => response.status === 200,
    'upload has downloadUrl': response => Boolean(response.json('downloadUrl'))
  });

  const downloadUrl = upload.json('downloadUrl');
  const tokenPart = downloadUrl.split('/').pop();
  const metadata = http.get(`${apiUrl}/download/${tokenPart}`);

  check(metadata, {
    'metadata status 200': response => response.status === 200
  });

  sleep(1);
}
