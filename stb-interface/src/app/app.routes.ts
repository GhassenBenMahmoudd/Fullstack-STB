import { Routes } from '@angular/router';
import { DeclarationFormComponent } from './components/declaration-form/declaration-form.component';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './pages/login/login.component';
import { AuthGuard } from './guards/auth.guard';
import { DeclarationListComponent } from './components/declaration-list/declaration-list.component';

export const routes: Routes = [
  { 
    path: 'login', 
    component: LoginComponent 
  },
  { 
    path: 'home', // Une route explicite pour la page d'accueil
    component: HomeComponent 
  },

  // --- Routes Protégées (nécessitent une connexion) ---
  { 
    // LA ROUTE MANQUANTE : Affiche la liste des déclarations
    path: 'declarations', 
    component: DeclarationListComponent,
    canActivate: [AuthGuard] // Protégez cette route !
  },
  { 
    path: 'declarations/new', 
    component: DeclarationFormComponent,
    canActivate: [AuthGuard] // Protégez cette route !
  },
  { 
    path: 'declarations/edit/:id', 
    component: DeclarationFormComponent,
    canActivate: [AuthGuard] // Protégez cette route !
  },

  // --- Redirections (toujours à la fin) ---
  { 
    // 1. Redirection de la racine : si l'utilisateur arrive sur le site,
    //    on l'envoie par défaut vers la liste des déclarations.
    path: '', 
    redirectTo: '/declarations', 
    pathMatch: 'full' 
  },
  { 
    // 2. Redirection "catch-all" : si l'URL est inconnue,
    //    on la renvoie aussi vers la liste des déclarations (ou une page 404).
    path: '**', 
    redirectTo: '/declarations' 
  }
];