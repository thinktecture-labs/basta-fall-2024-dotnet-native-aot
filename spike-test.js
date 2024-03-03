import http from 'k6/http';
import { check } from 'k6';

export const options = {
    vus: 15,
    duration: '30s',
    insecureSkipTLSVerify: true
};

export default function callToDosApi()
{
    const url = 'http://localhost:5000/todos';
    const result = http.get(url);
    check(result, {
        'OK': (r) => r.status === 200
    });
}