import axios from 'axios';
// use axios for automatic token injection on every request and to handle 401 responses globally (e.g. token expiration)
export const createAuthenticatedClient = (getAccessToken: () => string | undefined) => {
    const client = axios.create();

    client.interceptors.request.use(config => {
        const token = getAccessToken();
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    });

    client.interceptors.response.use(
        response => response,
        error => {
            if (error.response?.status === 401) {
                // Token expired — react-oidc-context will silently renew
                console.warn('Unauthorized — token may have expired');
            }
            return Promise.reject(error);
        }
    );

    return client;
};