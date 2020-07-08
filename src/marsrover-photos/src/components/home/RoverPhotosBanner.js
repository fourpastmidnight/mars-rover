import React from 'react';
import { Carousel } from 'react-bootstrap';
import PropTypes from 'prop-types';
import moment from 'moment';


const RoverPhotosBanner = ({id, carouselPhotoUrls, getPhotoRoverName}) => {
    function getPhotoDate(photoUrl) {
        const endOfDateIndex = photoUrl.lastIndexOf('/') - 1;
        return moment(photoUrl.substr(endOfDateIndex - 10, endOfDateIndex), "YYYY-MM-DD").format('LL');
    }

    return (
        <Carousel id={id} fade style={{backgroundColor: "#282828"}}>
            {carouselPhotoUrls.map(u => (
                <Carousel.Item key={u} className="d-flex justify-content-center align-items-center">
                    <img src={u} alt="" className="align-self-center" style={{maxHeight: "600px", maxWidth: "800px"}} />
                    <Carousel.Caption>
                        <h3>{getPhotoRoverName(u)}</h3>
                        <p>{getPhotoDate(u)}</p>
                    </Carousel.Caption>
                </Carousel.Item>
            ))}
        </Carousel>
    );
}

RoverPhotosBanner.propTypes = {
    id: PropTypes.string,
    carouselPhotoUrls: PropTypes.array.isRequired
}

export default RoverPhotosBanner;