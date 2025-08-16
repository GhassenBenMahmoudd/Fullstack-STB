import { Component, OnInit } from '@angular/core';
import { DeclarationCorruptionDto } from '../models/DeclarationCorruptionDto';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DeclarationCorruptionService } from '../services/declaration-corruption.service';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';

@Component({
  selector: 'app-declaration-corruption',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    HttpClientModule
  ],
  templateUrl: './declaration-corruption.component.html',
  styleUrls: ['./declaration-corruption.component.css']
})
export class DeclarationCorruptionComponent implements OnInit {

  declarations: DeclarationCorruptionDto[] = [];
  createForm: FormGroup;

  typesDomaines = ['Finance', 'RH', 'Opérations', 'Autre'];
  selectedFiles: File[] = [];
  loading = false;
  errorMessage = '';

  chooseAnonymeMode: boolean | null = null; // null = pas choisi, true = anonyme, false = connecté

  isLoggedIn = false;
  userId: number | null = null;
  userName: string = '';

  constructor(
    private service: DeclarationCorruptionService,
    private fb: FormBuilder
  ) {
    this.createForm = this.fb.group({
      idUser: ['', Validators.required],
      objetSignalement: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['', [Validators.required, Validators.maxLength(1000)]],
      entitesConcernees: ['', Validators.maxLength(200)],
      dateObservation: ['', Validators.required],
      typeDomaine: ['', Validators.required],
      statut: ['EnAttente']
      // Note : suppression du champ anonyme ici car choix séparé
    });
  }

  ngOnInit(): void {
    this.loadDeclarations();

    // Simule la récupération utilisateur connecté (à adapter)
    const user = JSON.parse(localStorage.getItem('currentUser') || '{}');
    if (user && user.id) {
      this.isLoggedIn = true;
      this.userId = user.id;
      this.userName = user.name || '';
    }
  }

  chooseAnonyme(choice: boolean) {
    if (choice === false && !this.isLoggedIn) {
      alert('Vous devez être connecté pour déclarer non anonymement.');
      // Exemple : redirection vers login
      // this.router.navigate(['/login']);
      return;
    }
    this.chooseAnonymeMode = choice;

    if (choice === false && this.userId) {
      this.createForm.patchValue({ idUser: this.userId });
      this.createForm.get('idUser')?.disable();
    } else {
      this.createForm.get('idUser')?.enable();
      this.createForm.patchValue({ idUser: '' });
    }
  }

  loadDeclarations(): void {
    this.service.getAll().subscribe({
      next: (data) => this.declarations = data,
      error: () => this.errorMessage = 'Erreur lors du chargement des déclarations'
    });
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files) {
      this.selectedFiles = Array.from(input.files);
    }
  }

  submit(): void {
    if (this.createForm.invalid) {
      this.errorMessage = 'Formulaire invalide, veuillez vérifier les champs.';
      return;
    }

    this.loading = true;
    this.errorMessage = '';

    const formValue = { ...this.createForm.value };

    // Supprime idUser si anonyme
    if (this.chooseAnonymeMode) {
      delete formValue.idUser;
    }

    const formData = new FormData();
    Object.entries(formValue).forEach(([key, value]) => {
      if (value !== null && value !== undefined) {
        formData.append(key, value.toString());
      }
    });

    this.selectedFiles.forEach(file => {
      formData.append('files', file, file.name);
    });

    this.service.create(formData).subscribe({
      next: () => {
        this.loading = false;
        this.createForm.reset();
        this.selectedFiles = [];
        this.chooseAnonymeMode = null;
        this.loadDeclarations();
      },
      error: () => {
        this.loading = false;
        this.errorMessage = 'Erreur lors de la création de la déclaration';
      }
    });
  }

  deleteDeclaration(id: number): void {
    if (!confirm('Confirmez-vous la suppression ?')) return;

    this.service.delete(id).subscribe({
      next: () => this.loadDeclarations(),
      error: () => alert('Erreur lors de la suppression')
    });
  }
}
