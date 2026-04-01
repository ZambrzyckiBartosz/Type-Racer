import React, { useState } from 'react';

function Auth({ onLoginSuccess }) {
    const [isLoginMode, setIsLoginMode] = useState(true);
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [message, setMessage] = useState('');

    const handleSubmit = async (e) => {
        e.preventDefault();
        setMessage('Sending...');
        
        const endpoint = isLoginMode ? '/api/login' : '/api/register';
        const url = `http://localhost:8080${endpoint}`; 
        
        try {
            const response = await fetch(url, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ username, password }),
            });

            if (response.ok) {
                if (isLoginMode) {
                    const data = await response.json();

                    console.log("data server: ", data);

					localStorage.setItem('token',data.token || data.Token);
					localStorage.setItem('username', data.username || data.Username);
                    
                    setMessage(`Logged as ${data.username}`); 
                    setTimeout(() => onLoginSuccess(data.username), 1000);
                } else {
                    setMessage('Account created, please log in');
                    setIsLoginMode(true);
                    setPassword('');
                }
            } else {
                const errorText = await response.text();
                let parsedError = errorText;
                try {
                    const jsonError = JSON.parse(errorText);
                    parsedError = jsonError.title || errorText;
                } catch {}
                setMessage('Error: ' + parsedError);
            }
        } catch (error) {
            setMessage("Connection error: Is backend running?");
        }
    };

    return (
        <div style={{ padding: '20px', backgroundColor: '#2a2a35', color: 'white', borderRadius: '8px', maxWidth: '300px', margin: '40px auto', textAlign: 'center', fontFamily: 'sans-serif' }}>
            <h2>{isLoginMode ? 'Login' : 'Register'}</h2>
            <form onSubmit={handleSubmit}>
                <input
                    type="text"
                    placeholder="Player name"
                    value={username}
                    onChange={(e) => setUsername(e.target.value)}
                    required
                    style={{ display: 'block', margin: '10px 0',color:'#000',backgroundColor: '#FFF', padding: '10px', width: '100%', boxSizing: 'border-box', borderRadius: '4px', border: 'none' }}
                />
                <input
                    type="password"
                    placeholder="Password"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    required
                    style={{ display: 'block', margin: '10px 0',color:'#000',backgroundColor: '#FFF',  padding: '10px', width: '100%', boxSizing: 'border-box', borderRadius: '4px', border: 'none' }}
                />
                <button type="submit" style={{ padding: '10px', width: '100%', cursor: 'pointer', backgroundColor: isLoginMode ? '#2196f3' : '#4CAF50', color: 'white', border: 'none', borderRadius: '4px', fontWeight: 'bold' }}>
                    {isLoginMode ? 'Log In' : 'Create Account'}
                </button>
            </form>
            {message && <p style={{ marginTop: '15px', color: '#ffb86c' }}>{message}</p>}
            <div style={{ marginTop: '20px', borderTop: '1px solid #444', paddingTop: '15px' }}>
                <button 
                    onClick={() => {
                        setIsLoginMode(!isLoginMode);
                        setMessage('');
                    }}
                    style={{ background: 'none', border: 'none', color: '#ff9800', cursor: 'pointer', textDecoration: 'underline' }}
                >
                    {isLoginMode ? 'Need an account? Register' : 'Have an account? Log in'}
                </button>
            </div>
        </div>
    );
}

export default Auth;
