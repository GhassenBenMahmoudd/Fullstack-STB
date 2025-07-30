import { Routes } from '@angular/router';
import { DeclarationListComponent } from './components/declaration-list/declaration-list.component';
import { DeclarationFormComponent } from './components/declaration-form/declaration-form.component';
import { HomeComponent } from './home/home.component';

export const routes: Routes = [
  { path: '', component: HomeComponent }, // La route racine affiche maintenant le HomeComponent
  { path: 'declarations', component: DeclarationListComponent },
  { path: 'declarations/new', component: DeclarationFormComponent },
  { path: 'declarations/edit/:id', component: DeclarationFormComponent },
  { path: '', redirectTo: '/declarations', pathMatch: 'full' },
  { path: '**', redirectTo: '/declarations' } // Redirection pour les routes inconnues
];
