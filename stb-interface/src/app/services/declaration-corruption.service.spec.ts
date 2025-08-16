import { TestBed } from '@angular/core/testing';

import { DeclarationCorruptionService } from './declaration-corruption.service';

describe('DeclarationCorruptionService', () => {
  let service: DeclarationCorruptionService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DeclarationCorruptionService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
