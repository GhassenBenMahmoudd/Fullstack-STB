import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { DeclarationCadeauService } from '../../services/declaration-cadeau.service';
import { CommonModule } from '@angular/common';

// Define the Fichier interface if not imported from elsewhere
export interface Fichier {
  id?: number;
  nom: string;
  url?: string;
}

@Component({
  selector: 'app-declaration-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './declaration-form.component.html',
  styleUrls: ['./declaration-form.component.css']
})
export class DeclarationFormComponent implements OnInit {
  declarationForm: FormGroup;
  isEditMode = false;
  private currentId?: number;
  selectedFiles: File[] = [];

 fichiersExistants: Fichier[] = [];

  typesRelation = ['PARTENAIRE', 'FOURNISSEUR', 'AUTRE'];

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private declarationService: DeclarationCadeauService
  ) {
    this.declarationForm = this.fb.group({
      idUser: [1, Validators.required], // À remplacer par l'ID de l'utilisateur connecté
      valeurEstime: [null, [Validators.required, Validators.min(0.01)]],
      identiteDonneur: ['', Validators.required],
      typeRelation: ['', Validators.required],
      occasion: [''],
      honneur: [false],
      message: [''],
      statut: ['EN_ATTENTE', Validators.required],
      dateReceptionCadeaux: ['', Validators.required],
      anonyme: [false],
      description: ['']
    });
  }


ngOnInit(): void {
  const idParam = this.route.snapshot.paramMap.get('id');
  if (idParam) {
    this.isEditMode = true;
    this.currentId = +idParam;

    this.declarationService.getById(this.currentId).subscribe({
      next: (data) => {
        // Remplir les champs du formulaire avec les données textuelles
        this.declarationForm.patchValue({
          valeurEstime: data.valeurEstime,
          identiteDonneur: data.identiteDonneur,
          typeRelation: data.typeRelation,
          occasion: data.occasion,
          honneur: data.honneur,
          message: data.message,
          statut: data.statut,
          dateReceptionCadeaux: data.dateReceptionCadeaux,
          anonyme: data.anonyme,
          description: data.description
        });

        // Stocker les fichiers pour affichage
        this.fichiersExistants = data.fichiers || [];
      },
      error: (err) => {
        console.error('Erreur lors du chargement de la déclaration :', err);
      }
    });
  }
}

onFileSelected(event: Event): void {
  const input = event.target as HTMLInputElement;
  if (input.files) {
    this.selectedFiles = Array.from(input.files);
  }
}

 onSubmit(): void {
  if (this.declarationForm.invalid) return;

  const formData = new FormData();

  // 1. Ajouter les données de la déclaration sous forme JSON stringifiée
  const cadeauDto = { ...this.declarationForm.value };
  formData.append('cadeauDto', JSON.stringify(cadeauDto));

  // 2. Ajouter les fichiers sélectionnés sous le champ 'newFiles'
  this.selectedFiles.forEach(file => {
    formData.append('newFiles', file); // 'newFiles' pour chaque fichier
  });

  // 3. Ajouter les IDs des fichiers existants à garder sous 'existingFileIds'
  this.fichiersExistants.forEach(fichier => {
    if (fichier.id) {
      formData.append('existingFileIds', fichier.id.toString());
    }
  });

  // 4. Appeler la méthode update ou create selon le mode
  const action = this.isEditMode && this.currentId
    ? this.declarationService.updateWithFiles(this.currentId, formData)
    : this.declarationService.createWithFiles(formData);

  action.subscribe({
    next: () => {
      alert(`Déclaration ${this.isEditMode ? 'mise à jour' : 'créée'} avec succès !`);
      this.router.navigate(['/declarations']);
    },
    error: (err) => {
      console.error('Erreur lors de l\'envoi', err);
      alert('Erreur lors de la soumission. Vérifiez la console.');
    }
  });
}

}
