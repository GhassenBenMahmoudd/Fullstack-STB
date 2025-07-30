// Interface pour les données reçues de l'API (DTO de lecture)
export interface DeclarationCadeau {
  idCadeaux: number;
  idUser: number;
  guid: string;
  valeurEstime: number;
  identiteDonneur: string;
  typeRelation: string;
  occasion?: string;
  honneur: boolean;
  dateDeclaration: string;
  message?: string;
  statut: string;
  dateReceptionCadeaux: string;
  anonyme: boolean;
  description?: string;
  archived: boolean;
}

// Type pour les données envoyées à l'API (DTO de création/mise à jour)
export type DeclarationCadeauPayload = Omit<DeclarationCadeau, 'idCadeaux' | 'guid' | 'dateDeclaration' | 'archived'>;
