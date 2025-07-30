import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { DeclarationCadeauService } from '../../services/declaration-cadeau.service';
import { CommonModule } from '@angular/common';

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
      this.declarationService.getById(this.currentId).subscribe(data => {
        this.declarationForm.patchValue(data);
      });
    }
  }

  onSubmit(): void {
    if (this.declarationForm.invalid) {
      return;
    }

    const formData = this.declarationForm.value;

    const action = this.isEditMode && this.currentId
      ? this.declarationService.update(this.currentId, formData)
      : this.declarationService.create(formData);

    action.subscribe(() => {
      alert(`Déclaration ${this.isEditMode ? 'mise à jour' : 'créée'} avec succès !`);
      this.router.navigate(['/declarations']);
    });
  }
}
