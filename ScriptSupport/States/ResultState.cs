using System.Windows.Documents;
using ICSharpCode.AvalonEdit.Document;
using Scrapiyard.Core.Models;
using ScriptSupport.Models;
using ScriptSupport.ViewModels;
using ScriptSupport.Collections;

namespace ScriptSupport.States
{
    public class ResultState : BaseViewModel
    {
        private TextDocument _resultCardDocument = new TextDocument();
        public TextDocument ResultCardDocument
        {
            get => _resultCardDocument;
            set => SetProperty(ref _resultCardDocument, value);
        }
        private FlowDocument _resultCardDocument1 = new FlowDocument();
        public FlowDocument ResultCardDocument1
        {
            get => _resultCardDocument1;
            set => SetProperty(ref _resultCardDocument1, value);
        }

        private TextDocument _resultScrapiyardDocument = new TextDocument();
        public TextDocument ResultScrapiyardDocument
        {
            get => _resultScrapiyardDocument;
            set => SetProperty(ref _resultScrapiyardDocument, value);
        }

        private List<ulong> _resultID = new();
        public List<ulong> ResultID
        {
            get => _resultID;
            set => SetProperty(ref _resultID, value);
        }

        private BulkObservableCollection<CardText>? _resultCardTexts = new();
        public BulkObservableCollection<CardText>? ResultCardTexts
        {
            get => _resultCardTexts;
            set => SetProperty(ref _resultCardTexts, value);
        }

        private BulkObservableCollection<FileItem>? _resultImageCards = new();
        public BulkObservableCollection<FileItem>? ResultImageCards
        {
            get => _resultImageCards;
            set => SetProperty(ref _resultImageCards, value);
        }

        private CardData? _resultCardData = new CardData();
        public CardData? ResultCardData
        {
            get => _resultCardData;
            set => SetProperty(ref _resultCardData, value);
        }

        private CompletionSymbol? _resultScrapiyard = new CompletionSymbol();
        public CompletionSymbol? ResultScrapiyard
        {
            get => _resultScrapiyard;
            set => SetProperty(ref _resultScrapiyard, value);
        }

        public ResultState()
        {

        }
    }
}
