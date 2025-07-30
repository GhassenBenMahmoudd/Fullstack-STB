import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AboutComponent } from '../about/about.component';
import { ConditionComponent } from '../condition/condition.component';
import { ParticiperComponent } from '../participer/participer.component';



@Component({
  selector: 'app-home',
  standalone: true,
  // On déclare ici tous les components que le template va utiliser
  imports: [
    CommonModule,
    AboutComponent,
    ConditionComponent,
    ParticiperComponent],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {
  // Pour une page d'accueil simple, il n'y a souvent pas besoin de logique ici.
  // On pourrait y ajouter des données si nécessaire.
  constructor() { }
}
