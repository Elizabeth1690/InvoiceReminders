db = db.getSiblingDB('invoice_reminders');

db.clients.insertMany([
    { _id: 'acme', name: 'Acme S.A.S.', email: 'contacto@acme.test' },
    { _id: 'globex', name: 'Globex Ltda.', email: 'pagos@globex.test' },
    { _id: 'initech', name: 'Initech Colombia', email: 'tesoreria@initech.test' }
]);

db.invoices.insertMany([
    { _id: 'inv-1001', clientId: 'acme', number: 'F-1001', amount: NumberDecimal('1500000'), dueDate: ISODate('2026-10-01T00:00:00Z'), status: 'primerrecordatorio' },
    { _id: 'inv-1002', clientId: 'acme', number: 'F-1002', amount: NumberDecimal('2300000'), dueDate: ISODate('2026-09-15T00:00:00Z'), status: 'segundorecordatorio' },
    { _id: 'inv-1003', clientId: 'acme', number: 'F-1003', amount: NumberDecimal('800000'), dueDate: ISODate('2026-08-10T00:00:00Z'), status: 'desactivado' },

    { _id: 'inv-2001', clientId: 'globex', number: 'F-2001', amount: NumberDecimal('4100000'), dueDate: ISODate('2026-10-05T00:00:00Z'), status: 'primerrecordatorio' },
    { _id: 'inv-2002', clientId: 'globex', number: 'F-2002', amount: NumberDecimal('950000'), dueDate: ISODate('2026-10-12T00:00:00Z'), status: 'primerrecordatorio' },
    { _id: 'inv-2003', clientId: 'globex', number: 'F-2003', amount: NumberDecimal('1750000'), dueDate: ISODate('2026-09-20T00:00:00Z'), status: 'segundorecordatorio' },

    { _id: 'inv-3001', clientId: 'initech', number: 'F-3001', amount: NumberDecimal('3200000'), dueDate: ISODate('2026-09-28T00:00:00Z'), status: 'segundorecordatorio' },
    { _id: 'inv-3002', clientId: 'initech', number: 'F-3002', amount: NumberDecimal('620000'), dueDate: ISODate('2026-10-18T00:00:00Z'), status: 'primerrecordatorio' },
    { _id: 'inv-3003', clientId: 'initech', number: 'F-3003', amount: NumberDecimal('1100000'), dueDate: ISODate('2026-07-30T00:00:00Z'), status: 'desactivado' }
]);

db.invoices.createIndex({ clientId: 1, status: 1, _id: 1 });