import { Injectable } from '@angular/core';
import { DeclarationCorruptionDto } from '../models/DeclarationCorruptionDto';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class DeclarationCorruptionService {
  private apiUrl = 'https://localhost:7048/api/DeclarationCorruption';

  constructor(private http: HttpClient) {}

  getAll(): Observable<DeclarationCorruptionDto[]> {
    return this.http.get<DeclarationCorruptionDto[]>(this.apiUrl);
  }

  create(formData: FormData): Observable<DeclarationCorruptionDto> {
    return this.http.post<DeclarationCorruptionDto>(`${this.apiUrl}/create-with-files`, formData);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
