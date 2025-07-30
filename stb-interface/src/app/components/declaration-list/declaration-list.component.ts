import { Component, OnInit } from '@angular/core';
import { DeclarationCadeauService } from '../../services/declaration-cadeau.service';
import { DeclarationCadeau } from '../../models/declaration-cadeau.model';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-declaration-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './declaration-list.component.html',
  styleUrls: ['./declaration-list.component.css']
})
export class DeclarationListComponent implements OnInit {
  declarations: DeclarationCadeau[] = [];

  constructor(private declarationService: DeclarationCadeauService) { }

  ngOnInit(): void {
    this.loadDeclarations();
  }

  loadDeclarations(): void {
    this.declarationService.getAll().subscribe(data => {
      this.declarations = data;
    });
  }

  deleteDeclaration(id: number): void {
    if (confirm('Êtes-vous sûr de vouloir supprimer cette déclaration ?')) {
      this.declarationService.delete(id).subscribe(() => {
        alert('Déclaration supprimée avec succès !');
        this.declarations = this.declarations.filter(d => d.idCadeaux !== id);
      });
    }
  }
}
