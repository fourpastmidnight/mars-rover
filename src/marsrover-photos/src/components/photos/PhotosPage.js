import React, { useState, useEffect } from 'react';
import PropTypes from 'prop-types';
import { Card, Col, Container, Row } from 'react-bootstrap';
import Lightbox from 'react-image-lightbox';
import { RingLoader } from 'react-spinners';
import 'react-image-lightbox/style.css';
import * as Api from '../../api/marsRoverPhotosApi'

const PhotosPage = ({rover, date}) => {
    const [isLoading, setIsLoading] = useState(true);
    const [photos, setPhotos] = useState([]);

    const handleOnClick = (imageIndex) => {
        setLightboxState({showLightbox: !lightboxState.showLightbox, imageIndex: imageIndex});
    }

    useEffect(() => {
        const fetchPhotos = async () => {
            setIsLoading(true);
            
            const haveRover = rover !== undefined && rover !== null && rover !== '';
            const haveDate = date !== undefined && date !== null;

            if (haveRover && haveDate) {
                setPhotos((await Api.getRoverPhotosByRoverOnDate(rover, date)).photos);
            } else if (haveRover) {
                setPhotos((await Api.getAllRoverPhotos(rover)).photos);
            } else if (haveDate) {
                setPhotos((await Api.getAllRoverPhotosOnDate(date)).photos);
            }

            setIsLoading(false);
        };

        fetchPhotos();
    }, [rover, date])

    const [lightboxState, setLightboxState] = useState({showLightbox: false, imageIndex: -1});

    return (
        <>
            <Container as="main" className="d-flex flex-column h-100">
                {isLoading
                    ? (
                        <Row className="flex-grow-1" style={{backgroundColor: "#fff8f0"}}>
                            <Col className="mt-4 justify-content-center">
                                <RingLoader loading={isLoading} size={200} className='align-self-center' />
                            </Col>
                        </Row>
                    ) : photos.length === 0
                        ? (
                            <Row className="flex-grow-1" style={{backgroundColor: "#fff8f0"}}>
                                <Col className="mt-4">
                                    <h1>No Photos</h1>
                                    <p>No photos could be found for the Mars {rover.substr(0, 1).toUpperCase() + rover.substr(1)} rover{date !== undefined ? " or there are no photos on the specified date" : ""}.</p>
                                </Col>
                            </Row>
                        ) : (
                            <Row className="row-cols-1 row-cols-md-2 row-cols-lg-3 flex-grow-1" style={{backgroundColor: "#fff8f0"}}>
                                {photos.map((p, i) => (
                                    <Col key={i} className="mt-4">
                                        <Card>
                                            <Card.Body>
                                                <div className="w-100" style={{backgroundColor: "transparent"}}>
                                                    <button onClick={() => handleOnClick(i)} style={{border: 'none', backgroundColor: 'transparent'}} >
                                                        <Card.Img variant="top" src={p} style={{height: "300px", width: "300px", objectFit: "none"}} />
                                                    </button>
                                                </div>
                                            </Card.Body>
                                        </Card>
                                    </Col>
                                ))}
                            </Row>
                        )
                }
            </Container>
            {lightboxState.showLightbox && (
                <Lightbox
                    mainSrc={photos[lightboxState.imageIndex]}
                    nextSrc={photos[(lightboxState.imageIndex + 1) % photos.length]}
                    prevSrc={photos[(lightboxState.imageIndex + photos.length - 1) % photos.length]}
                    onCloseRequest={() => setLightboxState({...lightboxState, showLightbox: !lightboxState.showLightbox})}
                    onMovePrevRequest={() => setLightboxState({...lightboxState, imageIndex: (lightboxState.imageIndex + photos.length - 1) % photos.length})}
                    onMoveNextRequest={() => setLightboxState({...lightboxState, imageIndex: (lightboxState.imageIndex + 1) % photos.length})}
                />
            )}
        </>
    );
}

PhotosPage.propTypes = {
    rover: PropTypes.string,
    date: PropTypes.instanceOf(Date)
}

export default PhotosPage;