export interface User {
  id: string;
  name: string;
  email: string;
  role: string;
  location: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  expiresIn: number;
  tokenType: string;
}

export interface RegisterRequest {
  name: string;
  email: string;
  password: string;
  location: string;
}