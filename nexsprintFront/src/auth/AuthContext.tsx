import React, {
  createContext,
  useContext,
  useState,
  useEffect,
  useRef,
  PropsWithChildren,
} from 'react';

interface AuthContextType {
  user: string | null;
  login: (email: string) => void;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType>({} as AuthContextType);

const SESSION_DURATION = 30 * 60 * 1000;
const USER_KEY = 'bytelib_user';
const LOGIN_TIME_KEY = 'bytelib_loginTime';

export const AuthProvider: React.FC<PropsWithChildren<{}>> = ({ children }) => {
  const [user, setUser] = useState<string | null>(null);
  const timeoutRef = useRef<number | null>(null);

  const handleLogout = () => {
    setUser(null);
    localStorage.removeItem(USER_KEY);
    localStorage.removeItem(LOGIN_TIME_KEY);
    if (timeoutRef.current) {
      clearTimeout(timeoutRef.current);
    }
  };

  const scheduleLogout = (delay: number) => {
    if (timeoutRef.current) clearTimeout(timeoutRef.current);
    timeoutRef.current = window.setTimeout(handleLogout, delay);
  };

  const handleLogin = (email: string) => {
    const now = Date.now();
    localStorage.setItem(USER_KEY, email);
    localStorage.setItem(LOGIN_TIME_KEY, now.toString());
    setUser(email);
    scheduleLogout(SESSION_DURATION);
  };

  useEffect(() => {
    const storedUser = localStorage.getItem(USER_KEY);
    const storedTime = localStorage.getItem(LOGIN_TIME_KEY);

    if (storedUser && storedTime) {
      const elapsed = Date.now() - parseInt(storedTime, 10);
      if (elapsed < SESSION_DURATION) {
        setUser(storedUser);
        scheduleLogout(SESSION_DURATION - elapsed);
      } else {
        handleLogout();
      }
    }

    return () => {
      if (timeoutRef.current) {
        clearTimeout(timeoutRef.current);
      }
    };
  }, []);

  return (
    <AuthContext.Provider
      value={{ user, login: handleLogin, logout: handleLogout }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => useContext(AuthContext);
