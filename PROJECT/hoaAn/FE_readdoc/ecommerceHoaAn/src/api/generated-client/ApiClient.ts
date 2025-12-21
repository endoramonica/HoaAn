/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { BaseHttpRequest } from './core/BaseHttpRequest';
import type { OpenAPIConfig } from './core/OpenAPI';
import { FetchHttpRequest } from './core/FetchHttpRequest';
import { AnalyticsService } from './services/AnalyticsService';
import { AuthService } from './services/AuthService';
import { CartService } from './services/CartService';
import { CategoryService } from './services/CategoryService';
import { CheckoutService } from './services/CheckoutService';
import { CommentsService } from './services/CommentsService';
import { CustomerAddressService } from './services/CustomerAddressService';
import { CustomerAdminService } from './services/CustomerAdminService';
import { ExplanationService } from './services/ExplanationService';
import { MixedFeedService } from './services/MixedFeedService';
import { OrderService } from './services/OrderService';
import { OtherService } from './services/OtherService';
import { PaymentWebhookService } from './services/PaymentWebhookService';
import { PostsService } from './services/PostsService';
import { ProductService } from './services/ProductService';
import { RecommendationService } from './services/RecommendationService';
import { UserPreferenceService } from './services/UserPreferenceService';
import { UsersService } from './services/UsersService';
import { WeatherForecastService } from './services/WeatherForecastService';
import { WishlistService } from './services/WishlistService';
type HttpRequestConstructor = new (config: OpenAPIConfig) => BaseHttpRequest;
export class ApiClient {
    public readonly analytics: AnalyticsService;
    public readonly auth: AuthService;
    public readonly cart: CartService;
    public readonly category: CategoryService;
    public readonly checkout: CheckoutService;
    public readonly comments: CommentsService;
    public readonly customerAddress: CustomerAddressService;
    public readonly customerAdmin: CustomerAdminService;
    public readonly explanation: ExplanationService;
    public readonly mixedFeed: MixedFeedService;
    public readonly order: OrderService;
    public readonly other: OtherService;
    public readonly paymentWebhook: PaymentWebhookService;
    public readonly posts: PostsService;
    public readonly product: ProductService;
    public readonly recommendation: RecommendationService;
    public readonly userPreference: UserPreferenceService;
    public readonly users: UsersService;
    public readonly weatherForecast: WeatherForecastService;
    public readonly wishlist: WishlistService;
    public readonly request: BaseHttpRequest;
    constructor(config?: Partial<OpenAPIConfig>, HttpRequest: HttpRequestConstructor = FetchHttpRequest) {
        this.request = new HttpRequest({
            BASE: config?.BASE ?? '',
            VERSION: config?.VERSION ?? '1',
            WITH_CREDENTIALS: config?.WITH_CREDENTIALS ?? false,
            CREDENTIALS: config?.CREDENTIALS ?? 'include',
            TOKEN: config?.TOKEN,
            USERNAME: config?.USERNAME,
            PASSWORD: config?.PASSWORD,
            HEADERS: config?.HEADERS,
            ENCODE_PATH: config?.ENCODE_PATH,
        });
        this.analytics = new AnalyticsService(this.request);
        this.auth = new AuthService(this.request);
        this.cart = new CartService(this.request);
        this.category = new CategoryService(this.request);
        this.checkout = new CheckoutService(this.request);
        this.comments = new CommentsService(this.request);
        this.customerAddress = new CustomerAddressService(this.request);
        this.customerAdmin = new CustomerAdminService(this.request);
        this.explanation = new ExplanationService(this.request);
        this.mixedFeed = new MixedFeedService(this.request);
        this.order = new OrderService(this.request);
        this.other = new OtherService(this.request);
        this.paymentWebhook = new PaymentWebhookService(this.request);
        this.posts = new PostsService(this.request);
        this.product = new ProductService(this.request);
        this.recommendation = new RecommendationService(this.request);
        this.userPreference = new UserPreferenceService(this.request);
        this.users = new UsersService(this.request);
        this.weatherForecast = new WeatherForecastService(this.request);
        this.wishlist = new WishlistService(this.request);
    }
}

