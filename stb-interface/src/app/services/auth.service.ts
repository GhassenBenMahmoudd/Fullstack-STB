import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';
import { jwtDecode } from 'jwt-decode'; // npm install jwt-decode
import { environment } from '../../environments/environment';
import { LoginDto } from '../models/LoginDto';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private tokenKey = 'auth_token';
  private apiUrl = `${environment.apiUrl}/Auth`;

  // BehaviorSubject pour suivre l'état de connexion en temps réel
  private userRoleSubject = new BehaviorSubject<string | null>(this.getRoleFromToken());
  public userRole$ = this.userRoleSubject.asObservable();

  private currentUserSubject = new BehaviorSubject<any>(this.getCurrentUser());
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient) { }

 getCurrentUser() {
  const token = this.getToken();
  if (!token) return null;

  try {
    const decoded: any = jwtDecode(token);
    return {
      id: decoded.id,
      prenom: decoded.prenom,
      nom: decoded.nom,
      email: decoded.email,
      role: decoded.role,
    };
  } catch (error) {
    console.error('Erreur décodage token dans getCurrentUser', error);
    return null;
  }
  
}


  // Méthode de connexion avec matricule (compatibilité)
  loginWithMatricule(matricule: string, password: string): Observable<any> {
    return this.login({ matricule, password });
  }

  // Méthode de connexion avec email
  loginWithEmail(email: string, password: string): Observable<any> {
    return this.login({ email, password });
  }

  getToken(): string | null {
  if (typeof window !== 'undefined' && window.localStorage) {
    return localStorage.getItem(this.tokenKey);
  }
  return null;
}

logout(): void {
  if (typeof window !== 'undefined' && window.localStorage) {
    localStorage.removeItem(this.tokenKey);
  }
  this.userRoleSubject.next(null);
  this.currentUserSubject.next(null);
}

login(loginData: LoginDto): Observable<any> {
  return this.http.post<any>(`${this.apiUrl}/login`, loginData).pipe(
    tap(response => {
      if (typeof window !== 'undefined' && window.localStorage) {
        localStorage.setItem(this.tokenKey, response.token);
      }
      this.userRoleSubject.next(this.getRoleFromToken());
      this.currentUserSubject.next(this.getCurrentUser());
    })
  );
}


  isLoggedIn(): boolean {
    const token = this.getToken();
    // Optionnel: vérifier l'expiration ici
    return !!token;
  }

  // Extraire le rôle du token
  getRoleFromToken(): string | null {
    const token = this.getToken();
    if (!token) return null;

    try {
      const decodedToken: any = jwtDecode(token);
      return decodedToken.role || decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || null;
    } catch (error) {
      console.error('Erreur lors du décodage du token', error);
      return null;
    }
  }

  // Méthodes pratiques pour vérifier le rôle
  isManager(): boolean {
    return this.getRoleFromToken() === 'Manager';
  }

  isEmploye(): boolean {
    return this.getRoleFromToken() === 'Employe';
  }

  isUser(): boolean {
    return this.getRoleFromToken() === 'User';
  }

  hasRole(role: string): boolean {
    return this.getRoleFromToken() === role;
  }

  hasAnyRole(roles: string[]): boolean {
    const userRole = this.getRoleFromToken();
    return userRole ? roles.includes(userRole) : false;
  }


  isAuthenticated(): boolean {
    return !!this.getToken();
  }
}
