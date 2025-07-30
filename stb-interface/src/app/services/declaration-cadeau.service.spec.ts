import { TestBed } from '@angular/core/testing';

import { DeclarationCadeauService } from './declaration-cadeau.service';

describe('DeclarationCadeauService', () => {
  let service: DeclarationCadeauService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DeclarationCadeauService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
