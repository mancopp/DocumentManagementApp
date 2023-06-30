using System;
using System.Collections.Generic;
using DocumentManagementApp.Models.Entities;

namespace DocumentManagementApp.Models
{
    public class DocumentManager
    {
        private readonly DatabaseContext _context;

        public DocumentManager(string connectionString)
        {
            _context = new DatabaseContext(connectionString);
        }

        public void ImportData(List<Document> documents, List<DocumentItem> documentItems)
        {
            foreach (var document in documents)
            {
                _context.AddDocument(document);
            }

            foreach (var documentItem in documentItems)
            {
                _context.AddDocumentItem(documentItem);
            }
        }

        public IEnumerable<JoinedDocument> GetJoinedDocuments()
        {
            return _context.GetJoinedDocuments();
        }
    }
}
