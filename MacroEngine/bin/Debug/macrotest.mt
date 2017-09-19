#macro(coloredHeader $color)
    <tr>
        <th style="background-color:$color;height:100px;border-bottom:8px solid \#8cd5f2;text-align:left;padding-left:35px;">
            <font face="Arial">
                <h2 style="color:\#8cd5f2;margin:0;padding-top:25px;font-size:18pt;">Some Spamer Company</h2>
            </font>
        </th>
    </tr>
#end

#macro(templateHeader $moduleName $emailTitle)
   <!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1\.0 Transitional//EN" "http://www\.w3\.org/TR/xhtml1/DTD/xhtml1-transitional\.dtd">
    <html xmlns="http://www\.w3\.org/1999/xhtml">
        <body style="background-color:\#eeeeee;margin:0">
            <table cellpadding="0" cellspacing="0" style="width:100%;font-family:Arial;background-color: \#FDFDFD;border-collapse: collapse;margin:0 auto;table-layout:fixed">
                <thead>
                    #coloredHeader("#015cac")
                    <tr>
                        <td style="text-align:left;padding-left:35px;padding-top:25px;">
                            <h2 style="text-align:left;color:\#cc0000;margin:0;font-size:16pt;">
                                <font face="Arial">
                                    $moduleName
                                </font>
                            </h2>
                            <h2 style="text-align:left;color:\#000;margin:0;padding-top:5px;font-size:23pt;">
                                <font face="Arial">
                                    $emailTitle
                                </font>
                            </h2>
                        </td>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td style="padding-left: 35px; padding-right: 35px; padding-top: 15px; padding-bottom: 15px; font-size:13pt;">
                        <!-- template header end -->
#end
#macro(templateFooter)
                        <!-- template footer start -->
                        </td>
                    </tr>
                </tbody>
                <tfoot>
                    <tr>
                        <td style="padding-left:35px;text-align:left;height:60px;background-color:\#999999;color:\#FFF;font-size:13pt;">
                            <font face="Arial">
                                &copy; 2017 Super Spamer, Inc\. \$
                            </font>
                         </td>
                    </tr>
                </tfoot>
            </table>
        </body>
    </html>
#end

#templateHeader("SYSTEM SECURITY" "Account Locked")
<table>
    <tbody>
        <tr>
            <td>
                <font face="Arial">
                    Dear $User.FirstName $User.LastName,
                    <br/>
                    <br/>
                    You have exceeded the maximum number of failed login attempts (5 attempts) and your account has been locked\.
                    <br/>
                    Your administrator has been notified and may select to unblock your account\.
                    <br/>
                    #if($User.VerifiedEmail)
                        <br/>
                        You can use next link to change password:
                        <br/>
                        <a href="$User.RegistrationLink">$User.RegistrationLink</a>
                        <br/>
                    #end
                    <br/>
                    If you require immediate assistance, please contact your administrator or call Support at 123-295-1000\.
                    <br/>
                    <br/>
                    Thanks,
                    <br/>
                    Super Spamer, Inc\.
                </font>
            </td>
        </tr>
    </tbody>
</table>
#templateFooter()

#if($User.Order.OrderItemsList.Count > 0)
<table>
<thead>
	<th>Name</th>
	<th>Count</th>
	<th>Price</th>
</thead>
#foreach($order in $User.Order.OrderItemsList)<tr><td>$order.Name</td><td>$order.Count</td><td>$order.Price</td></tr>
#end
</table>
#else No orders
#end