import { Fichier } from "./Fichier";

export interface DeclarationCorruptionDto {
  idCorruption: number;
  idUser: number;
  objetSignalement: string;
  description: string;
  entitesConcernees?: string;
  dateObservation: string;
  typeDomaine: string;
  statut: string;
  anonyme: boolean;
fichiers: Fichier[]; 
}