namespace SistemaInventarioApi.Common
{
    public static class ErrorCodes
    {
        public const string ValidationFailed = "VALIDATION_FAILED";
        public const string InvalidRequest = "INVALID_REQUEST";

        public static class Auth
        {
            public const string InvalidCredentials =
                "AUTH_INVALID_CREDENTIALS";

            public const string UserDisabled =
                "AUTH_USER_DISABLED";

            public const string TokenInvalid =
                "AUTH_TOKEN_INVALID";

            public const string TokenExpired =
                "AUTH_TOKEN_EXPIRED";

            public const string AccessDenied =
                "AUTH_ACCESS_DENIED";
        }

        public static class Users
        {
            public const string NotFound =
                "USER_NOT_FOUND";

            public const string EmailAlreadyExists =
                "USER_EMAIL_ALREADY_EXISTS";

            public const string CannotDeleteCurrentUser =
                "USER_CANNOT_DELETE_CURRENT_USER";

            public const string HasReferences =
                "USER_HAS_REFERENCES";
        }

        public static class Categories
        {
            public const string NotFound =
                "CATEGORY_NOT_FOUND";

            public const string HasProducts =
                "CATEGORY_HAS_PRODUCTS";
        }

        public static class Suppliers
        {
            public const string NotFound =
                "SUPPLIER_NOT_FOUND";

            public const string HasProducts =
                "SUPPLIER_HAS_PRODUCTS";
        }

        public static class Products
        {
            public const string NotFound =
                "PRODUCT_NOT_FOUND";

            public const string InsufficientStock =
                "PRODUCT_INSUFFICIENT_STOCK";

            public const string InvalidStock =
                "PRODUCT_INVALID_STOCK";
        }

        public static class Customers
        {
            public const string NotFound =
                "CUSTOMER_NOT_FOUND";

            public const string HasSales =
                "CUSTOMER_HAS_SALES";
        }

        public static class Sales
        {
            public const string NotFound =
                "SALE_NOT_FOUND";

            public const string EmptyDetails =
                "SALE_EMPTY_DETAILS";

            public const string InvalidQuantity =
                "SALE_INVALID_QUANTITY";
        }

        public static class Infrastructure
        {
            public const string DatabaseError =
                "DATABASE_ERROR";

            public const string InternalError =
                "INTERNAL_ERROR";
        }

    }
}
