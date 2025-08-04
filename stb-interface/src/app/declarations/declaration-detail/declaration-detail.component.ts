import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common'; // ✅ nécessaire pour pipes
import { DeclarationCadeauService } from '../../services/declaration-cadeau.service';
import { DeclarationCadeau } from '../../models/declaration-cadeau.model';

@Component({
  selector: 'app-declaration-detail',
  standalone: true, // ✅
  imports: [CommonModule], // ✅ ajout du CommonModule ici
  templateUrl: './declaration-detail.component.html',
  styleUrls: ['./declaration-detail.component.css']
})
export class DeclarationDetailComponent implements OnInit {
  declaration?: DeclarationCadeau;

  constructor(
    private route: ActivatedRoute,
    private declarationService: DeclarationCadeauService
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.declarationService.getById(id).subscribe(data => {
      this.declaration = data;
    });
  }
}
