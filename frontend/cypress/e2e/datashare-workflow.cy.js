describe('DataShare workflow', () => {
  it('creates an account, uploads a file, lists it and downloads it', () => {
    const email = `e2e.${Date.now()}@example.com`;
    const password = 'Password123!';

    cy.visit('/register');
    cy.get('[data-cy="register-email"]').type(email);
    cy.get('[data-cy="register-password"]').type(password);
    cy.get('[data-cy="register-confirm"]').type(password);
    cy.get('[data-cy="register-submit"]').click();
    cy.location('pathname').should('eq', '/upload');

    cy.get('[data-cy="upload-file"]').selectFile('cypress/fixtures/sample.txt', { force: true });
    cy.get('[data-cy="upload-expiration"]').select('7');
    cy.get('[data-cy="upload-submit"]').click();
    cy.get('[data-cy="download-url"]').should('contain.value', '/download/').invoke('val').as('downloadUrl');

    cy.contains('Mes fichiers').click();
    cy.contains('sample.txt').should('be.visible');

    cy.get('@downloadUrl').then(downloadUrl => {
      cy.intercept('POST', `${Cypress.env('apiUrl')}/download/**`).as('downloadFile');
      cy.visit(downloadUrl);
      cy.contains('Télécharger un fichier').should('be.visible');
      cy.contains('sample.txt').should('be.visible');
      cy.get('[data-cy="download-submit"]').click();
      cy.wait('@downloadFile').its('response.statusCode').should('eq', 200);
    });
  });

  it('shows an error when the download token is invalid', () => {
    cy.visit('/download/token-invalide');
    cy.contains('Lien invalide').should('be.visible');
  });
});
