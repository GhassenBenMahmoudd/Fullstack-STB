import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DeclarationCadeau, DeclarationCadeauPayload } from '../models/declaration-cadeau.model';

@Injectable({
  providedIn: 'root'
} )
export class DeclarationCadeauService {
  // Adaptez le port si nécessaire
  private readonly apiUrl = 'https://localhost:7048/api/DeclarationCadeau';

  constructor(private http: HttpClient ) { }

  getAll(): Observable<DeclarationCadeau[]> {
    return this.http.get<DeclarationCadeau[]>(this.apiUrl );
  }

  getById(id: number): Observable<DeclarationCadeau> {
    return this.http.get<DeclarationCadeau>(`${this.apiUrl}/${id}` );
  }

  create(cadeau: DeclarationCadeauPayload): Observable<DeclarationCadeau> {
    return this.http.post<DeclarationCadeau>(this.apiUrl, cadeau );
  }

  update(id: number, cadeau: DeclarationCadeauPayload): Observable<DeclarationCadeau> {
    return this.http.put<DeclarationCadeau>(`${this.apiUrl}/${id}`, cadeau );
  }

  delete(id: number): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(`${this.apiUrl}/${id}` );
  }

  toggleArchive(id: number): Observable<any> {
    return this.http.patch(`${this.apiUrl}/${id}/toggle-archive`, {} );
  }
  updateDeclarationStatus(id: number, nouveauStatut: string): Observable<DeclarationCadeau> {
    return this.http.patch<DeclarationCadeau>(`${this.apiUrl}/${id}/statut`, { nouveauStatut } );
  }
}
