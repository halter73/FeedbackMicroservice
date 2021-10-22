### Example demo flow

```
C:\dev\halter73\FeedbackMicroservice> httprepl https://localhost:7060/feedback

https://localhost:7060/feedback/> post submit
HTTP/1.1 201 Created
Content-Type: application/json; charset=utf-8
Date: Fri, 22 Oct 2021 18:02:46 GMT
Location: /feedback/1
Server: Kestrel
Transfer-Encoding: chunked

{
  "id": 1,
  "rating": 5,
  "comment": "Wow!",
  "wasReviewed": false,
  "reviewNotes": null
}


https://localhost:7060/feedback/> post submit
HTTP/1.1 201 Created
Content-Type: application/json; charset=utf-8
Date: Fri, 22 Oct 2021 18:02:56 GMT
Location: /feedback/2
Server: Kestrel
Transfer-Encoding: chunked

{
  "id": 2,
  "rating": 1,
  "comment": "Stop nagging me for feedback!",
  "wasReviewed": false,
  "reviewNotes": null
}


https://localhost:7060/feedback/> get pending
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8
Date: Fri, 22 Oct 2021 18:03:10 GMT
Server: Kestrel
Transfer-Encoding: chunked

[
  {
    "id": 1,
    "rating": 5,
    "comment": "Wow!",
    "wasReviewed": false,
    "reviewNotes": null
  },
  {
    "id": 2,
    "rating": 1,
    "comment": "Stop nagging me for feedback!",
    "wasReviewed": false,
    "reviewNotes": null
  }
]


https://localhost:7060/feedback/> post review/1
HTTP/1.1 200 OK
Content-Length: 0
Date: Fri, 22 Oct 2021 18:03:44 GMT
Server: Kestrel


https://localhost:7060/feedback/> post review/2
HTTP/1.1 200 OK
Content-Length: 0
Date: Fri, 22 Oct 2021 18:04:53 GMT
Server: Kestrel


https://localhost:7060/feedback/> get 1
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8
Date: Fri, 22 Oct 2021 18:05:01 GMT
Server: Kestrel
Transfer-Encoding: chunked

{
  "id": 1,
  "rating": 5,
  "comment": "Wow!",
  "wasReviewed": true,
  "reviewNotes": null
}


https://localhost:7060/feedback/> get 2
HTTP/1.1 200 OK
Content-Type: application/json; charset=utf-8
Date: Fri, 22 Oct 2021 18:05:03 GMT
Server: Kestrel
Transfer-Encoding: chunked

{
  "id": 2,
  "rating": 1,
  "comment": "Stop nagging me for feedback!",
  "wasReviewed": true,
  "reviewNotes": "Reduce feedback prompts by 50%."
}
```