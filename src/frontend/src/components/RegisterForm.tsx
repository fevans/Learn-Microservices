import { useState } from 'react';
import { useAuth } from 'react-oidc-context';
import axios from 'axios';

const IDENTITY_URL = process.env.REACT_APP_IDENTITY_SERVICE_URL;

interface RegisterDto {
    username:    string;
    email:       string;
    password:    string;
    startingGil: number;
}

export const RegisterForm = () => {
    const auth = useAuth();
    const [form, setForm] = useState<RegisterDto>({
        username:    '',
        email:       '',
        password:    '',
        startingGil: 0,
    });
    const [error,   setError]   = useState<string | null>(null);
    const [loading, setLoading] = useState(false);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setLoading(true);
        setError(null);

        try {
            // Call the public registration endpoint — no auth token needed
            await axios.post(`${IDENTITY_URL}/Account/register`, form);

            // Redirect to IdentityServer login so the user can sign in immediately
            await auth.signinRedirect({
                extraQueryParams: { login_hint: form.email },
            });
        } catch (err: any) {
            const message = err.response?.data?.message ?? 'Registration failed';
            setError(message);
        } finally {
            setLoading(false);
        }
    };

    return (
        <form onSubmit={handleSubmit}>
            <h2>Create Account</h2>

            {error && <p style={{ color: 'red' }}>{error}</p>}

            <label>
                Username
                <input
                    type="text"
                    value={form.username}
                    onChange={e => setForm(p => ({ ...p, username: e.target.value }))}
                    required
                />
            </label>

            <label>
                Email
                <input
                    type="email"
                    value={form.email}
                    onChange={e => setForm(p => ({ ...p, email: e.target.value }))}
                    required
                />
            </label>

            <label>
                Password
                <input
                    type="password"
                    value={form.password}
                    onChange={e => setForm(p => ({ ...p, password: e.target.value }))}
                    minLength={8}
                    required
                />
            </label>

            <button type="submit" disabled={loading}>
                {loading ? 'Creating account...' : 'Register'}
            </button>

            <p>
                Already have an account?{' '}
                <button type="button" onClick={() => auth.signinRedirect()}>
                    Sign In
                </button>
            </p>
        </form>
    );
};