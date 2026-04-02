import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { User, LoginRequest, AuthResponse } from '../models/user.model';
import { environment } from 'src/environments/environment';
import { jwtDecode } from 'jwt-decode';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly TOKEN_KEY = 'dm_token';
  private readonly USER_KEY  = 'dm_user';

  private currentUserSubject = new BehaviorSubject<User | null>(
    this.loadUserFromStorage()
  );

  currentUser$: Observable<User | null> = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient, private router: Router) {}

  getCurrentUser(): User | null {
    return this.currentUserSubject.value;
  }

  login(dto: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/auth/login`, dto).pipe(
      tap(response => {
        localStorage.setItem(this.TOKEN_KEY, response.token);
        const payload: any = this.decodeToken(response.token);
        console.log(payload);
        const user: User = {
          id: payload.sub,
          name: payload.name,
          email: payload.email,
          role: payload.role,
          location: payload.location
        };
        this.currentUserSubject.next(user);
      })
    );
  }

  private loadUserFromStorage(): User | null {
    const raw = localStorage.getItem(this.USER_KEY);
    return raw ? JSON.parse(raw) : null;
  }

  private decodeToken(token: string): any {
    return jwtDecode(token);
  }
}