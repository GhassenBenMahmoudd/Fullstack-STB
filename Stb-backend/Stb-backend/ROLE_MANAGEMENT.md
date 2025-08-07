# Gestion des Rôles et Permissions - STB Backend

## Vue d'ensemble

Le système de gestion des rôles a été amélioré pour afficher clairement le rôle de chaque utilisateur connecté et limiter la déclaration des cadeaux aux employés uniquement.

## Rôles Disponibles

### 1. Manager
- **Description**: Accès complet à toutes les fonctionnalités
- **Permissions**:
  - ✅ Déclarer des cadeaux
  - ✅ Voir toutes les déclarations
  - ✅ Modifier le statut des déclarations
  - ✅ Archiver/désarchiver les déclarations
  - ✅ Supprimer les déclarations
  - ✅ Voir les rapports
  - ✅ Gérer les utilisateurs

### 2. Employé
- **Description**: Peut déclarer des cadeaux et consulter ses déclarations
- **Permissions**:
  - ✅ Déclarer des cadeaux
  - ❌ Voir toutes les déclarations (seulement les siennes)
  - ❌ Modifier le statut des déclarations
  - ❌ Archiver/désarchiver les déclarations
  - ❌ Supprimer les déclarations
  - ❌ Voir les rapports
  - ❌ Gérer les utilisateurs

### 3. Utilisateur Standard
- **Description**: Accès limité
- **Permissions**:
  - ❌ Déclarer des cadeaux
  - ❌ Voir les déclarations
  - ❌ Toutes les autres fonctionnalités

## Endpoints API

### Authentification

#### POST `/api/auth/login`
- **Description**: Connexion utilisateur
- **Réponse**: Token JWT + informations utilisateur + rôle + permissions

```json
{
  "token": "jwt_token_here",
  "user": {
    "id": 1,
    "name": "John Doe",
    "email": "john@example.com",
    "matricule": "EMP001",
    "role": "Employe",
    "roleDescription": "Employé - Peut déclarer des cadeaux et consulter ses déclarations",
    "permissions": {
      "canDeclareGifts": true,
      "canViewAllDeclarations": false,
      "canUpdateStatus": false,
      "canArchive": false,
      "canDelete": false,
      "canViewReports": false
    }
  },
  "message": "Connexion réussie. Rôle: Employe"
}
```

#### GET `/api/auth/me`
- **Description**: Obtenir les informations de l'utilisateur connecté
- **Authentification**: Requise
- **Réponse**: Informations utilisateur + rôle + permissions

### Gestion des Rôles

#### GET `/api/role/permissions`
- **Description**: Obtenir les permissions de l'utilisateur connecté
- **Authentification**: Requise

#### GET `/api/role/roles`
- **Description**: Liste de tous les rôles disponibles
- **Authentification**: Requise (Manager uniquement)

#### GET `/api/role/can-declare-gifts`
- **Description**: Vérifier si l'utilisateur peut déclarer des cadeaux
- **Authentification**: Requise

## Déclaration des Cadeaux

### Restrictions par Rôle

#### Employés et Managers
- ✅ Peuvent créer des déclarations de cadeaux
- ✅ Peuvent voir leurs propres déclarations
- ✅ Peuvent modifier leurs propres déclarations

#### Managers uniquement
- ✅ Peuvent voir toutes les déclarations
- ✅ Peuvent modifier le statut des déclarations
- ✅ Peuvent archiver/désarchiver les déclarations
- ✅ Peuvent supprimer les déclarations

## Logging des Activités

Le système enregistre automatiquement toutes les activités des utilisateurs avec :
- Nom de l'utilisateur
- Rôle de l'utilisateur
- Action effectuée
- Timestamp
- Adresse IP

### Affichage en Console

```
=== ACTIVITÉ UTILISATEUR ===
Utilisateur: John Doe
Rôle: Employe
Email: john@example.com
Matricule: EMP001
Action: POST /api/declarationcadeau
Statut: 201
=============================
```

## Middleware

### UserActivityMiddleware
- Enregistre toutes les activités des utilisateurs
- Affiche les informations de rôle en console
- Log les requêtes anonymes et authentifiées

## Sécurité

### JWT Claims
Le token JWT contient les claims suivants :
- `sub` (Subject): ID de l'utilisateur
- `name`: Nom complet de l'utilisateur
- `role`: Rôle de l'utilisateur
- `email`: Email de l'utilisateur
- `matricule`: Matricule (si applicable)

### Autorisations
- Les endpoints sont protégés par des attributs `[Authorize]`
- Les rôles spécifiques sont vérifiés avec `[Authorize(Roles = "Manager,Employe")]`
- Les permissions sont vérifiées au niveau de l'application

## Utilisation

1. **Connexion**: L'utilisateur se connecte via `/api/auth/login`
2. **Rôle affiché**: Le système affiche automatiquement le rôle dans la réponse
3. **Permissions**: Les permissions sont incluses dans la réponse de connexion
4. **Activité**: Toutes les actions sont loggées avec le rôle de l'utilisateur
5. **Restrictions**: Seuls les employés et managers peuvent déclarer des cadeaux

## Exemples d'Utilisation

### Connexion d'un Employé
```bash
POST /api/auth/login
{
  "email": "employe@stb.com",
  "password": "password123"
}
```

**Réponse**:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "user": {
    "role": "Employe",
    "roleDescription": "Employé - Peut déclarer des cadeaux et consulter ses déclarations",
    "permissions": {
      "canDeclareGifts": true,
      "canViewAllDeclarations": false
    }
  }
}
```

### Vérification des Permissions
```bash
GET /api/role/can-declare-gifts
Authorization: Bearer <token>
```

**Réponse**:
```json
{
  "userName": "John Doe",
  "role": "Employe",
  "canDeclareGifts": true,
  "message": "John Doe peut déclarer des cadeaux"
}
``` 