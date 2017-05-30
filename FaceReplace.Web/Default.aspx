<%@ Page Language="C#" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <!-- The above 3 meta tags *must* come first in the head; any other head content must come *after* these tags -->
    <meta name="description" content="">
    <meta name="author" content="">
    <link rel="icon" href="../../favicon.ico">

    <title>FaceReplace</title>

    <!-- Latest compiled and minified CSS -->
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css" integrity="sha384-BVYiiSIFeK1dGmJRAkycuHAHRg32OmUcww7on3RYdg4Va+PmSTsz/K68vbdEjh4u" crossorigin="anonymous">

    <!-- Optional theme -->
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap-theme.min.css" integrity="sha384-rHyoN1iRsVXV4nD0JutlnGaslCJuC7uwjduW9SVrLvRYooPp2bWYgmgJQIXwl/Sp" crossorigin="anonymous">

    <!-- Custom styles for this template -->
    <link href="carousel.css" rel="stylesheet">
</head>
<!-- NAVBAR
================================================== -->
<body>
    <div class="navbar-wrapper">
        <div class="container">

            <nav class="navbar navbar-inverse navbar-static-top">
                <div class="container">
                    <div class="navbar-header">
                        <a class="navbar-brand" href="#">FaceReplace</a>
                    </div>
                </div>
            </nav>

        </div>
    </div>

    <!-- Carousel
================================================== -->
    <div id="myCarousel" class="carousel slide" data-ride="carousel">
        <!-- Indicators -->
        <ol class="carousel-indicators">
            <li data-target="#myCarousel" data-slide-to="0" class="active"></li>
            <li data-target="#myCarousel" data-slide-to="1"></li>
            <li data-target="#myCarousel" data-slide-to="2"></li>
        </ol>
        <div class="carousel-inner" role="listbox">
            <div class="item active">
                <img class="first-slide" src="data:image/gif;base64,R0lGODlhAQABAIAAAHd3dwAAACH5BAAAAAAALAAAAAABAAEAAAICRAEAOw==" alt="First slide">
                <div class="container">
                    <div class="carousel-caption">
                        <h1>This could be you!</h1>
                        <img src="<%=ConfigurationManager.AppSettings["ApiLocation"]%>HttpBackgroundImageRetrieve/0" height="250" />
                    </div>
                </div>
            </div>
            <div class="item">
                <img class="second-slide" src="data:image/gif;base64,R0lGODlhAQABAIAAAHd3dwAAACH5BAAAAAAALAAAAAABAAEAAAICRAEAOw==" alt="Second slide">
                <div class="container">
                    <div class="carousel-caption">
                        <h1>Or maybe this is more your style?</h1>
                        <img src="<%=ConfigurationManager.AppSettings["ApiLocation"]%>HttpBackgroundImageRetrieve/1" height="250" />
                    </div>
                </div>
            </div>
            <div class="item">
                <img class="third-slide" src="data:image/gif;base64,R0lGODlhAQABAIAAAHd3dwAAACH5BAAAAAAALAAAAAABAAEAAAICRAEAOw==" alt="Third slide">
                <div class="container">
                    <div class="carousel-caption">
                        <h1>Tempted?</h1>
                        <img src="<%=ConfigurationManager.AppSettings["ApiLocation"]%>HttpBackgroundImageRetrieve/1" height="250" />
                        <p><a class="btn btn-lg btn-primary" href="#" role="button">Let's do this!</a></p>
                    </div>
                </div>
            </div>
        </div>
        <a class="left carousel-control" href="#myCarousel" role="button" data-slide="prev">
            <span class="glyphicon glyphicon-chevron-left" aria-hidden="true"></span>
            <span class="sr-only">Previous</span>
        </a>
        <a class="right carousel-control" href="#myCarousel" role="button" data-slide="next">
            <span class="glyphicon glyphicon-chevron-right" aria-hidden="true"></span>
            <span class="sr-only">Next</span>
        </a>
    </div>
    <!-- /.carousel -->

    <hr class="featurette-divider">

    <div class="container">

        <!-- Three columns of text below the carousel -->
        <div class="row">
            <div class="col-lg-4">
                <h2>Upload your file here:</h2>
                <form id="upload-form" enctype="multipart/form-data">
                    <label class="control-label">Select File</label>
                    <input id="imageupload" name="imageupload" type="file" class="file">
                    <input id="doesntmatter" name="doesntmatter" type="hidden" value="yeah"/>
                    <input type="submit" value="submit" id="submit" />
                </form>
            </div>
            <!-- /.col-lg-4 -->
        </div>
        <!-- /.row -->

        <p></p>

        <div class="row">
            <div class="col-lg-4">
                <h2>Results will appear here:</h2>
                <img id="imgresult" height="400" />
            </div>
            <!-- /.col-lg-4 -->
        </div>

        <p></p>

        <!-- FOOTER -->
        <footer>
            <p class="pull-right"><a href="#">Back to top</a></p>
            <p>&copy; 2017 FaceReplace, Inc. &middot; <a href="#">Privacy</a> &middot; <a href="#">Terms</a></p>
        </footer>

    </div>
    <!-- /.container -->


    <!-- Bootstrap core JavaScript
================================================== -->
    <!-- Placed at the end of the document so the pages load faster -->
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.12.4/jquery.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js" integrity="sha384-Tc5IQib027qvyjSMfHjOMaLkfuWVxZxUPnCJA7l2mCWNIpG9mGCD8wGNIcPD7Txa" crossorigin="anonymous"></script>
    <script>
        $(function () {
            $('#upload-form').submit(function (e) {
                e.preventDefault(); // cancel the actual submit
                var form = $('#upload-form')[0]; // You need to use standard javascript object here
                var formData = new FormData(form);

                $.ajax(
                    {
                        url: "<%=ConfigurationManager.AppSettings["ApiLocation"]%>HttpForegroundImageUpload",
                        type: "POST",
                        data: formData,
                        dataType: 'json',
                        processData: false,
                        contentType: false,
                        success: function (response) {
                            if (response.IsSuccessful) {
                                var fileName = response.FileName.Name;
                                setTimeout(
                                    function() {imageStatusCheck(fileName)},
                                    3000
                                );
                            } else {
                                alert("Something went wrong with the upload: " + response.Message);
                            }
                        },
                        error: function (jqXHR, textStatus, errorMessage) {
                            alert(errorMessage);
                        }
                    }
                );
                return false;
            });
        });
		
		function imageStatusCheck(name) {
			var retrieveUrl = "<%=ConfigurationManager.AppSettings["ApiLocation"]%>HttpForegroundImageRetrieve/" + name + "/";
			$.ajax({
				url: retrieveUrl,
				complete: function(xhr, textStatus) {
					if (textStatus != 'success') {
						setTimeout(
							function () { imageStatusCheck(name) },
							2000
						);
                    }
				    debugger;
					$("#imgresult").attr("src",retrieveUrl);					
				}
			});
		};
		
    </script>

</body>
</html>
