import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';
import { jwtDecode } from 'jwt-decode'; // Installer avec: npm install jwt-decode
import { environment } from '../../environments/environment'; 

@Injectable({
  providedIn: 'root'
} )
export class AuthService {
  private apiUrl = `${environment.apiUrl}/Auth`;
  private tokenKey = 'auth_token';

  // BehaviorSubject pour suivre l'état de connexion en temps réel
  private userRoleSubject = new BehaviorSubject<string | null>(this.getRoleFromToken());
  public userRole$ = this.userRoleSubject.asObservable();

  constructor(private http: HttpClient ) { }

  login(matricule: string, password: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/login`, { matricule, password } ).pipe(
      tap(response => {
        // Stocker le token dans le localStorage
        localStorage.setItem(this.tokenKey, response.token);
        // Mettre à jour le rôle de l'utilisateur
        this.userRoleSubject.next(this.getRoleFromToken());
      })
    );
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    this.userRoleSubject.next(null); // Mettre à jour le rôle à null
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  isLoggedIn(): boolean {
    const token = this.getToken();
    // On peut aussi vérifier si le token est expiré ici
    return !!token;
  }

  // Méthode pour extraire le rôle du token
  getRoleFromToken(): string | null {
    const token = this.getToken();
    if (!token) {
      return null;
    }
    try {
      // Le token contient un payload avec les claims. Le rôle est souvent dans 'role'.
      // La clé doit correspondre exactement à celle définie dans l'API .NET (ClaimTypes.Role)
      const decodedToken: any = jwtDecode(token);
      return decodedToken.role || null;
    } catch (error) {
      console.error("Erreur lors du décodage du token", error);
      return null;
    }
  }

  // Raccourci pratique pour vérifier si l'utilisateur est un manager
  isManager(): boolean {
    return this.getRoleFromToken() === 'Manager';
  }
}
