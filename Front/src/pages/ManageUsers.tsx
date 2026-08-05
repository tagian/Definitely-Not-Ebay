import { useEffect, useState } from 'react';
import { listUsers, approveUser } from '../api/users';
import type { User } from '../api/types';

export default function ManageUsers() {
    const [users, setUsers] = useState < User[] > ([]);
    useEffect(() => {
        listUsers().then(setUsers);
    }, []);
    return (
        <div className="p-6">
            <h1 className="text-2xl font-bold mb-4">Users</h1>
            <table className="w-full border">
                <thead>
                    <tr className="bg-gray-50"><th className="p-2 text-left">Name</th>
                    <th className="p-2">Email</th>
                    <th className="p-2">Role</th>
                    <th className="p-2"></th></tr>
                </thead>
                <tbody>
                    {users.map(u=> (
                    <tr key={u.userId} className="border-t">
                        <td className="p-2">{u.name}</td>
                        <td className="p-2">{u.email}</td>
                        <td className="p-2">{u.role}</td>
                        <td className="p-2 text-right">
                            <button onClick={()=>approveUser(u.userId)} className="border px-3 py-1 rounded">Approve</button>
                        </td>
                    </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}
