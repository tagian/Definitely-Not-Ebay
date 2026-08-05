/* eslint-disable @typescript-eslint/no-explicit-any */
import { type FormEvent, useState} from 'react';
import { login, me } from '../api/auth';
import { useAuth } from '../store/authstore';
import { useNavigate } from 'react-router-dom';


export default function LoginPage() {
    const {setAuth} = useAuth();
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState < string | null > (null);
    const navigate = useNavigate();



    async function onSubmit(e: FormEvent) {
        e.preventDefault();
        setError(null);
        try {
            const res = await login(email, password);
            if (!res.isSuccess || !res.token) throw new Error(res.errorMessage || 'Login failed');
            setAuth(res.token, null);
            const profile = await me();
            setAuth(res.token, profile);
            navigate('/');
        } catch (err: any) {
            setError(err.message);
        }
}


    return (
        <div className="max-w-sm mx-auto p-6">
            <h1 className="text-2xl font-bold mb-4">Login</h1>
            <form onSubmit={onSubmit} className="space-y-3">
                <input className="w-full border p-2 rounded" placeholder="email" value={email} onChange={e=>setEmail(e.target.value)} />
                <input className="w-full border p-2 rounded" type="password" placeholder="password" value={password} onChange={e=>setPassword(e.target.value)} />
                {error && <div className="text-red-600 text-sm">{error}</div>}
                <button className="bg-black text-white px-4 py-2 rounded">Sign in</button>
            </form>
        </div>
    );
}
