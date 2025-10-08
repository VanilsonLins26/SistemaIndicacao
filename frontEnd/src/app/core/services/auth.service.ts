import { HttpClient } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { catchError, Observable, of, tap } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { LoginDto, LoginResponseDto, RegistrarDto, UsuarioDto } from '../models/auth.model';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);
  private apiUrl = environment.apiUrl;

  currentUser = signal<UsuarioDto | null | undefined>(undefined);
  isLoggedIn = computed(() => !!this.currentUser());

  constructor() { }

   public initAuthentication(): Observable<UsuarioDto | null> | null {
    const token = this.getAccessToken();
    if (token) {
      return this.getProfile();
    } else {
      this.currentUser.set(null);
      return null;
    }
  }


  getProfile(): Observable<UsuarioDto | null>{
    return this.http.get<UsuarioDto>(`${this.apiUrl}/autenticacao/perfil`).pipe(
      tap(user => {this.currentUser.set(user)
      }),
      catchError(() => {
        this.logout();
        return of(null);
      })
    );
  }

  login(dto: LoginDto): Observable<LoginResponseDto>{
    return this.http.post<LoginResponseDto>(`${this.apiUrl}/autenticacao/login`, dto).pipe(
      tap(response => {
         this.saveTokens(response.accessToken, response.refreshToken);
         this.currentUser.set(response.usuario);
         this.router.navigate(['/perfil']);
      })
    );
  }

  register(dto: RegistrarDto): Observable<LoginResponseDto>{
    return this.http.post<LoginResponseDto>(`${this.apiUrl}/autenticacao/registrar`, dto).pipe(
      tap(response => {
         this.saveTokens(response.accessToken, response.refreshToken);
         this.currentUser.set(response.usuario);
         this.router.navigate(['/perfil']);
      })
    );
    
  }

  logout(): void{
    localStorage.removeItem('access_token');
    localStorage.removeItem('refresh_token');
    this.currentUser.set(null);
    this.router.navigate(['/login']);
  }

  getAccessToken(): string | null{
    return localStorage.getItem('access_token');
  }

  getRefreshToken(): string | null{
    return localStorage.getItem('refresh_token');
  }

  private saveTokens(accessToken: string, refreshToken: string): void{
    localStorage.setItem('access_token', accessToken);
    localStorage.setItem('refresh_token', refreshToken);
  }
}
