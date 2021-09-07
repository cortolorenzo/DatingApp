namespace API.Helpers
{
    public class PaginationHeader
    {
        public PaginationHeader(int currentPage, int itemsPerPAge, int totalItems, int totalPages)
        {
            CurrentPage = currentPage;
            ItemsPerPAge = itemsPerPAge;
            TotalItems = totalItems;
            TotalPages = totalPages;
        }

        public int CurrentPage { get; set; }
        public int ItemsPerPAge { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }
}