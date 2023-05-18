#import <Foundation/Foundation.h>
#import <CoreTelephony/CTTelephonyNetworkInfo.h>
#import <CoreTelephony/CTCarrier.h>

@interface TeleClass:NSObject
- (NSString*)tele;
@end

@implementation TeleClass
- (NSString*) tele {
    CTTelephonyNetworkInfo *netinfo = [[CTTelephonyNetworkInfo alloc] init];
    CTCarrier *carrier = [netinfo subscriberCellularProvider];
    return [carrier isoCountryCode];
}
@end

char* MakeStringCopy (const char* string)
{
    if (string == NULL)
        return NULL;
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    return res;
}

extern "C" {
    const char* _ex_getCarrier() {
        TeleClass *teleClass = [[TeleClass alloc]init];
        NSString *ret = [teleClass tele];
        return MakeStringCopy([ret UTF8String]);
    }
}
